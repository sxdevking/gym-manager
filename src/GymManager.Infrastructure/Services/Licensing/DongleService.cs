using GymManager.Application.Common.Interfaces;
using HidSharp;
using Microsoft.Extensions.Logging;

namespace GymManager.Infrastructure.Services.Licensing;

/// <summary>
/// Servicio de comunicación con dongle USB usando HidSharp
/// </summary>
public class DongleService : IDongleService, IDisposable
{
    private readonly ILogger<DongleService> _logger;
    private readonly IEncryptionService _encryptionService;

    // Configuración del dongle (ajustar según el dispositivo real)
    private const int VendorId = 0x1234;   // ID del fabricante
    private const int ProductId = 0x5678;  // ID del producto

    private HidDevice? _currentDevice;
    private HidStream? _currentStream;
    private bool _isConnected;
    private readonly System.Timers.Timer _connectionTimer;

    public event EventHandler<bool>? DongleConnectionChanged;

    public DongleService(ILogger<DongleService> logger, IEncryptionService encryptionService)
    {
        _logger = logger;
        _encryptionService = encryptionService;

        // Timer para verificar conexión periódicamente
        _connectionTimer = new System.Timers.Timer(2000);
        _connectionTimer.Elapsed += (s, e) => CheckConnection();
        _connectionTimer.Start();
    }

    public bool IsDongleConnected()
    {
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