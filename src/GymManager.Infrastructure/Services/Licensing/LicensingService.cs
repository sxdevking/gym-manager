using GymManager.Application.Common.Interfaces;
using GymManager.Domain.Entities;
using GymManager.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

// Alias para resolver ambigüedad con QuestPDF
using LicenseType = GymManager.Domain.Enums.LicenseType;

namespace GymManager.Infrastructure.Services.Licensing;

/// <summary>
/// Servicio principal de gestión de licencias
/// </summary>
public class LicenseService : ILicenseService
{
    private readonly ILogger<LicenseService> _logger;
    private readonly IDongleService _dongleService;
    private readonly IEncryptionService _encryptionService;
    private readonly IConfiguration _configuration;
    private readonly GymDbContext _context;

    // Clave maestra para cifrado (en producción, guardar en lugar seguro)
    private readonly string _masterKey;

    // Modo desarrollo
    private readonly bool _developmentMode;

    // Cache de licencia para no validar en cada operación
    private LicenseInfo? _cachedLicense;
    private DateTime _lastValidation = DateTime.MinValue;
    private readonly TimeSpan _validationInterval = TimeSpan.FromMinutes(30);

    public LicenseService(
        ILogger<LicenseService> logger,
        IDongleService dongleService,
        IEncryptionService encryptionService,
        IConfiguration configuration,
        GymDbContext context)
    {
        _logger = logger;
        _dongleService = dongleService;
        _encryptionService = encryptionService;
        _configuration = configuration;
        _context = context;

        _masterKey = _configuration["Licensing:MasterKey"]
            ?? throw new InvalidOperationException("Licensing:MasterKey no configurada");

        // ═══════════════════════════════════════════════════════════════
        // MODO DESARROLLO - Leer de configuración
        // ═══════════════════════════════════════════════════════════════
        var devModeStr = _configuration["Licensing:DevelopmentMode"];
        _developmentMode = !string.IsNullOrEmpty(devModeStr) &&
                          (devModeStr.Equals("true", StringComparison.OrdinalIgnoreCase) || devModeStr == "1");

        if (_developmentMode)
        {
            _logger.LogWarning("══════════════════════════════════════════════════════════");
            _logger.LogWarning("   🔓 LICENSE SERVICE EN MODO DESARROLLO");
            _logger.LogWarning("   ⚠️  Licencia automática sin validación de dongle");
            _logger.LogWarning("   📝 Para producción: DevelopmentMode = false");
            _logger.LogWarning("══════════════════════════════════════════════════════════");
        }
    }

    public bool IsDonglePresent()
    {
        // En modo desarrollo, siempre presente
        if (_developmentMode) return true;

        return _dongleService.IsDongleConnected();
    }

    public async Task<LicenseValidationResult> ValidateLicenseAsync()
    {
        // ═══════════════════════════════════════════════════════════════
        // BYPASS DESARROLLO - Licencia automática
        // ═══════════════════════════════════════════════════════════════
        if (_developmentMode)
        {
            _logger.LogDebug("Validación en modo desarrollo - Licencia automática");

            // Crear licencia de desarrollo en caché
            _cachedLicense = new LicenseInfo(
                LicenseId: Guid.Parse("00000000-0000-0000-0000-000000000001"),
                LicenseKey: "DEV-MODE-LICENSE",
                HardwareId: "DEVELOPMENT-MODE",
                Type: LicenseType.ENTERPRISE,
                MaxBranches: 99,
                MaxUsers: 999,
                IssuedAt: DateTime.UtcNow.AddYears(-1),
                ExpiresAt: null, // Perpetua
                IsActive: true
            );
            _lastValidation = DateTime.UtcNow;

            return new LicenseValidationResult(
                IsValid: true,
                Message: "Licencia de desarrollo activa",
                LicenseType: LicenseType.ENTERPRISE,
                MaxBranches: 99,
                MaxUsers: 999,
                DaysRemaining: null
            );
        }

        // ═══════════════════════════════════════════════════════════════
        // CÓDIGO ORIGINAL DE PRODUCCIÓN
        // ═══════════════════════════════════════════════════════════════
        try
        {
            _logger.LogInformation("Iniciando validación de licencia...");

            // 1. Verificar que el dongle está conectado
            if (!_dongleService.IsDongleConnected())
            {
                _logger.LogWarning("Dongle no detectado");
                return new LicenseValidationResult(
                    IsValid: false,
                    Message: "Dispositivo de seguridad no detectado. Por favor conecte el dongle USB."
                );
            }

            // 2. Obtener Hardware ID del dongle
            var hardwareId = _dongleService.GetHardwareId();
            if (string.IsNullOrEmpty(hardwareId))
            {
                _logger.LogWarning("No se pudo obtener Hardware ID");
                return new LicenseValidationResult(
                    IsValid: false,
                    Message: "No se pudo leer el dispositivo de seguridad."
                );
            }

            // 3. Buscar licencia en base de datos
            var license = await _context.Licenses
                .FirstOrDefaultAsync(l => l.HardwareId == hardwareId && l.IsActive);

            if (license == null)
            {
                _logger.LogWarning("No se encontró licencia para Hardware ID: {HardwareId}", hardwareId[..16]);
                return new LicenseValidationResult(
                    IsValid: false,
                    Message: "Licencia no encontrada. Por favor active el software."
                );
            }

            // 4. Verificar expiración
            if (license.ExpiresAt.HasValue && license.ExpiresAt.Value < DateTime.UtcNow)
            {
                _logger.LogWarning("Licencia expirada: {ExpiresAt}", license.ExpiresAt);
                return new LicenseValidationResult(
                    IsValid: false,
                    Message: $"La licencia expiró el {license.ExpiresAt:dd/MM/yyyy}. Por favor renueve su licencia."
                );
            }

            // 5. Validar integridad de la licencia
            var expectedHash = GenerateLicenseHash(license.LicenseId, hardwareId, license.LicenseType);
            var storedKey = _encryptionService.Decrypt(license.LicenseKey, _masterKey);

            if (expectedHash != storedKey)
            {
                _logger.LogError("Integridad de licencia comprometida");
                return new LicenseValidationResult(
                    IsValid: false,
                    Message: "Error de validación. Contacte a soporte técnico."
                );
            }

            // 6. Actualizar última validación
            license.LastValidation = DateTime.UtcNow;
            license.ValidationCount++;
            await _context.SaveChangesAsync();

            // 7. Cachear resultado
            _cachedLicense = new LicenseInfo(
                LicenseId: license.LicenseId,
                LicenseKey: license.LicenseKey,
                HardwareId: license.HardwareId,
                Type: license.LicenseType,
                MaxBranches: license.MaxBranches,
                MaxUsers: license.MaxUsers,
                IssuedAt: license.IssuedAt,
                ExpiresAt: license.ExpiresAt,
                IsActive: license.IsActive
            );
            _lastValidation = DateTime.UtcNow;

            // 8. Calcular días restantes
            int? daysRemaining = null;
            if (license.ExpiresAt.HasValue)
            {
                daysRemaining = (int)(license.ExpiresAt.Value - DateTime.UtcNow).TotalDays;
            }

            _logger.LogInformation("Licencia válida. Tipo: {Type}, Expira: {Expires}",
                license.LicenseType, license.ExpiresAt?.ToString("dd/MM/yyyy") ?? "Perpetua");

            return new LicenseValidationResult(
                IsValid: true,
                Message: "Licencia válida",
                LicenseType: license.LicenseType,
                MaxBranches: license.MaxBranches,
                MaxUsers: license.MaxUsers,
                DaysRemaining: daysRemaining
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error durante validación de licencia");
            return new LicenseValidationResult(
                IsValid: false,
                Message: "Error al validar licencia. Contacte a soporte técnico."
            );
        }
    }

    public async Task<LicenseValidationResult> ActivateLicenseAsync(string licenseKey)
    {
        // ═══════════════════════════════════════════════════════════════
        // BYPASS DESARROLLO - Activación automática
        // ═══════════════════════════════════════════════════════════════
        if (_developmentMode)
        {
            _logger.LogWarning("Activación en modo desarrollo - Automática");
            return new LicenseValidationResult(
                IsValid: true,
                Message: "Licencia de desarrollo activada automáticamente",
                LicenseType: LicenseType.ENTERPRISE,
                MaxBranches: 99,
                MaxUsers: 999
            );
        }

        // ═══════════════════════════════════════════════════════════════
        // CÓDIGO ORIGINAL DE PRODUCCIÓN
        // ═══════════════════════════════════════════════════════════════
        try
        {
            _logger.LogInformation("Iniciando activación de licencia...");

            // 1. Verificar dongle
            if (!_dongleService.IsDongleConnected())
            {
                return new LicenseValidationResult(
                    IsValid: false,
                    Message: "Conecte el dongle USB antes de activar."
                );
            }

            // 2. Obtener Hardware ID
            var hardwareId = _dongleService.GetHardwareId();
            if (string.IsNullOrEmpty(hardwareId))
            {
                return new LicenseValidationResult(
                    IsValid: false,
                    Message: "No se pudo leer el dispositivo de seguridad."
                );
            }

            // 3. Verificar si ya existe licencia para este dongle
            var existingLicense = await _context.Licenses
                .FirstOrDefaultAsync(l => l.HardwareId == hardwareId);

            if (existingLicense != null)
            {
                return new LicenseValidationResult(
                    IsValid: false,
                    Message: "Este dispositivo ya tiene una licencia activada."
                );
            }

            // 4. Decodificar y validar clave de licencia
            var licenseData = DecodeLicenseKey(licenseKey);
            if (licenseData == null)
            {
                return new LicenseValidationResult(
                    IsValid: false,
                    Message: "Clave de licencia inválida."
                );
            }

            // 5. Crear nueva licencia
            var newLicense = new License
            {
                LicenseId = Guid.NewGuid(),
                HardwareId = hardwareId,
                LicenseType = licenseData.Value.Type,
                MaxBranches = licenseData.Value.MaxBranches,
                MaxUsers = licenseData.Value.MaxUsers,
                IssuedAt = DateTime.UtcNow,
                ExpiresAt = licenseData.Value.ExpiresAt,
                IsActive = true,
                ValidationCount = 1,
                LastValidation = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };

            // 6. Generar y cifrar license key
            var hash = GenerateLicenseHash(newLicense.LicenseId, hardwareId, newLicense.LicenseType);
            newLicense.LicenseKey = _encryptionService.Encrypt(hash, _masterKey);

            // 7. Guardar en base de datos
            _context.Licenses.Add(newLicense);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Licencia activada exitosamente. ID: {LicenseId}", newLicense.LicenseId);

            return new LicenseValidationResult(
                IsValid: true,
                Message: "Licencia activada exitosamente",
                LicenseType: newLicense.LicenseType,
                MaxBranches: newLicense.MaxBranches,
                MaxUsers: newLicense.MaxUsers
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error durante activación de licencia");
            return new LicenseValidationResult(
                IsValid: false,
                Message: "Error al activar licencia. Contacte a soporte técnico."
            );
        }
    }

    public async Task<LicenseInfo?> GetCurrentLicenseAsync()
    {
        // Usar cache si es reciente
        if (_cachedLicense != null &&
            DateTime.UtcNow - _lastValidation < _validationInterval)
        {
            return _cachedLicense;
        }

        var result = await ValidateLicenseAsync();
        return result.IsValid ? _cachedLicense : null;
    }

    public async Task<bool> CanAddBranchAsync()
    {
        var license = await GetCurrentLicenseAsync();
        if (license == null) return false;

        var currentBranches = await _context.Branches
            .CountAsync(b => b.IsActive && b.DeletedAt == null);

        return currentBranches < license.MaxBranches;
    }

    public async Task<bool> CanAddUserAsync()
    {
        var license = await GetCurrentLicenseAsync();
        if (license == null) return false;

        var currentUsers = await _context.Users
            .CountAsync(u => u.IsActive && u.DeletedAt == null);

        return currentUsers < license.MaxUsers;
    }

    /// <summary>
    /// Genera hash único para validación de licencia
    /// </summary>
    private string GenerateLicenseHash(Guid licenseId, string hardwareId, LicenseType type)
    {
        var dataToHash = $"{licenseId}|{hardwareId}|{type}|GymManager2024";
        return _encryptionService.GenerateHash(dataToHash);
    }

    /// <summary>
    /// Decodifica una clave de licencia proporcionada por el vendedor
    /// </summary>
    private (LicenseType Type, int MaxBranches, int MaxUsers, DateTime? ExpiresAt)? DecodeLicenseKey(string key)
    {
        try
        {
            // Formato esperado: BASE64(TYPE-BRANCHES-USERS-EXPIRY-CHECKSUM)
            var decoded = _encryptionService.Decrypt(key, _masterKey);
            var parts = decoded.Split('-');

            if (parts.Length < 4) return null;

            var type = Enum.Parse<LicenseType>(parts[0]);
            var branches = int.Parse(parts[1]);
            var users = int.Parse(parts[2]);
            DateTime? expiry = parts[3] == "PERPETUAL"
                ? null
                : DateTime.Parse(parts[3]);

            return (type, branches, users, expiry);
        }
        catch
        {
            return null;
        }
    }
}