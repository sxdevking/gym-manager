using GymManager.Application.Common.Interfaces;
using HidSharp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace GymManager.Infrastructure.Services.Licensing;

/// <summary>
/// Servicio de comunicación con dongle USB usando HidSharp
/// </summary>
public class DongleService : IDongleService, IDisposable
{
    private readonly ILogger<DongleService> _logger;
    private readonly IEncryptionService _encryptionService;
    private readonly bool _developmentMode;

    // Configuración del dongle (ajustar según el dispositivo real)
    private const int VendorId = 0x1234;   // ID del fabricante
    private const int ProductId = 0x5678;  // ID del producto

    // Hardware ID emulado para modo desarrollo
    private const string DevHardwareId = "DEV-MODE-GYMMANAGER-2024-HARDWARE-ID-EMULATED";

    private HidDevice? _currentDevice;
    private HidStream? _currentStream;
    private bool _isConnected;
    private readonly System.Timers.Timer _connectionTimer;

    public event EventHandler<bool>? DongleConnectionChanged;

    public DongleService(
        ILogger<DongleService> logger,
        IEncryptionService encryptionService,
        IConfiguration configuration)
    {
        _logger = logger;
        _encryptionService = encryptionService;

        // ═══════════════════════════════════════════════════════════════
        // MODO DESARROLLO - Leer de configuración
        // ═══════════════════════════════════════════════════════════════
        var devModeStr = configuration["Licensing:DevelopmentMode"];
        _developmentMode = !string.IsNullOrEmpty(devModeStr) &&
                          (devModeStr.Equals("true", StringComparison.OrdinalIgnoreCase) || devModeStr == "1");

        if (_developmentMode)
        {
            _logger.LogWarning("══════════════════════════════════════════════════════════");
            _logger.LogWarning("   🔧 DONGLE SERVICE EN MODO DESARROLLO");
            _logger.LogWarning("   ⚠️  El dongle USB NO será requerido");
            _logger.LogWarning("   📝 Para producción: DevelopmentMode = false");
            _logger.LogWarning("══════════════════════════════════════════════════════════");
            _isConnected = true;
        }

        // Timer para verificar conexión periódicamente
        _connectionTimer = new System.Timers.Timer(2000);
        _connectionTimer.Elapsed += (s, e) => CheckConnection();
        _connectionTimer.Start();
    }

    public bool IsDongleConnected()
    {
        // ═══════════════════════════════════════════════════════════════
        // BYPASS DESARROLLO
        // ═══════════════════════════════════════════════════════════════
        if (_developmentMode)
        {
            return true;
        }

        // ═══════════════════════════════════════════════════════════════
        // CÓDIGO ORIGINAL DE PRODUCCIÓN
        // ═══════════════════════════════════════════════════════════════
        try
        {
            var devices = DeviceList.Local.GetHidDevices(VendorId, ProductId);
            var connected = devices.Any();

            if (connected != _isConnected)
            {
                _isConnected = connected;
                DongleConnectionChanged?.Invoke(this, connected);
                _logger.LogInformation("Dongle {Status}", connected ? "conectado" : "desconectado");
            }

            return connected;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al verificar conexión del dongle");
            return false;
        }
    }

    public string? GetHardwareId()
    {
        // ═══════════════════════════════════════════════════════════════
        // BYPASS DESARROLLO
        // ═══════════════════════════════════════════════════════════════
        if (_developmentMode)
        {
            _logger.LogDebug("Retornando Hardware ID de desarrollo");
            return _encryptionService.GenerateHash(DevHardwareId);
        }

        // ═══════════════════════════════════════════════════════════════
        // CÓDIGO ORIGINAL DE PRODUCCIÓN
        // ═══════════════════════════════════════════════════════════════
        try
        {
            var device = DeviceList.Local.GetHidDevices(VendorId, ProductId).FirstOrDefault();

            if (device == null)
            {
                _logger.LogWarning("No se encontró dongle conectado");
                return null;
            }

            // Generar ID único basado en propiedades del dispositivo
            var uniqueString = $"{device.VendorID}-{device.ProductID}-{device.GetSerialNumber()}-{device.DevicePath}";
            var hardwareId = _encryptionService.GenerateHash(uniqueString);

            _logger.LogDebug("Hardware ID generado: {HardwareId}", hardwareId[..16] + "...");

            return hardwareId;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener Hardware ID del dongle");
            return null;
        }
    }

    public byte[]? ReadFromDongle()
    {
        // ═══════════════════════════════════════════════════════════════
        // BYPASS DESARROLLO
        // ═══════════════════════════════════════════════════════════════
        if (_developmentMode)
        {
            return null; // No hay datos que leer en modo desarrollo
        }

        // ═══════════════════════════════════════════════════════════════
        // CÓDIGO ORIGINAL DE PRODUCCIÓN
        // ═══════════════════════════════════════════════════════════════
        try
        {
            var device = DeviceList.Local.GetHidDevices(VendorId, ProductId).FirstOrDefault();

            if (device == null)
                return null;

            if (!device.TryOpen(out var stream))
            {
                _logger.LogWarning("No se pudo abrir conexión con el dongle");
                return null;
            }

            using (stream)
            {
                byte[] buffer = new byte[64];
                int bytesRead = stream.Read(buffer, 0, buffer.Length);

                if (bytesRead > 0)
                {
                    byte[] result = new byte[bytesRead];
                    Array.Copy(buffer, result, bytesRead);
                    return result;
                }
            }

            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al leer del dongle");
            return null;
        }
    }

    public bool WriteToDongle(byte[] data)
    {
        // ═══════════════════════════════════════════════════════════════
        // BYPASS DESARROLLO
        // ═══════════════════════════════════════════════════════════════
        if (_developmentMode)
        {
            _logger.LogDebug("Escritura simulada en modo desarrollo: {Bytes} bytes", data.Length);
            return true;
        }

        // ═══════════════════════════════════════════════════════════════
        // CÓDIGO ORIGINAL DE PRODUCCIÓN
        // ═══════════════════════════════════════════════════════════════
        try
        {
            var device = DeviceList.Local.GetHidDevices(VendorId, ProductId).FirstOrDefault();

            if (device == null)
                return false;

            if (!device.TryOpen(out var stream))
            {
                _logger.LogWarning("No se pudo abrir conexión con el dongle para escritura");
                return false;
            }

            using (stream)
            {
                stream.Write(data, 0, data.Length);
                return true;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al escribir en el dongle");
            return false;
        }
    }

    private void CheckConnection()
    {
        IsDongleConnected();
    }

    public void Dispose()
    {
        _connectionTimer?.Stop();
        _connectionTimer?.Dispose();
        _currentStream?.Dispose();
        GC.SuppressFinalize(this);
    }
}