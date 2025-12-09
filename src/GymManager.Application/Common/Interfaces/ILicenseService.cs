using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GymManager.Domain.Enums;

namespace GymManager.Application.Common.Interfaces;

/// <summary>
/// Resultado de validación de licencia
/// </summary>
public record LicenseValidationResult(
    bool IsValid,
    string Message,
    LicenseType? LicenseType = null,
    int? MaxBranches = null,
    int? MaxUsers = null,
    int? DaysRemaining = null
);

/// <summary>
/// Información de la licencia activa
/// </summary>
public record LicenseInfo(
    Guid LicenseId,
    string LicenseKey,
    string HardwareId,
    LicenseType Type,
    int MaxBranches,
    int MaxUsers,
    DateTime IssuedAt,
    DateTime? ExpiresAt,
    bool IsActive
);

/// <summary>
/// Servicio principal de licenciamiento
/// </summary>
public interface ILicenseService
{
    /// <summary>
    /// Valida la licencia actual
    /// </summary>
    Task<LicenseValidationResult> ValidateLicenseAsync();

    /// <summary>
    /// Activa una nueva licencia
    /// </summary>
    Task<LicenseValidationResult> ActivateLicenseAsync(string licenseKey);

    /// <summary>
    /// Obtiene información de la licencia actual
    /// </summary>
    Task<LicenseInfo?> GetCurrentLicenseAsync();

    /// <summary>
    /// Verifica si se puede agregar una nueva sucursal
    /// </summary>
    Task<bool> CanAddBranchAsync();

    /// <summary>
    /// Verifica si se puede agregar un nuevo usuario
    /// </summary>
    Task<bool> CanAddUserAsync();

    /// <summary>
    /// Verifica el estado del dongle
    /// </summary>
    bool IsDonglePresent();
}