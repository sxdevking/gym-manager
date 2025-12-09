using GymManager.Domain.Common;
using GymManager.Domain.Enums;

namespace GymManager.Domain.Entities;

/// <summary>
/// Entidad de Licencia - Controla el licenciamiento del software por dongle USB
/// </summary>
public class License : AuditableEntity
{
    /// <summary>
    /// Identificador único de la licencia
    /// </summary>
    public Guid LicenseId { get; set; }

    /// <summary>
    /// Clave de licencia cifrada
    /// </summary>
    public string LicenseKey { get; set; } = string.Empty;

    /// <summary>
    /// ID único del hardware del dongle (hash)
    /// </summary>
    public string HardwareId { get; set; } = string.Empty;

    /// <summary>
    /// Tipo de licencia (Trial, Standard, Enterprise)
    /// </summary>
    public LicenseType LicenseType { get; set; } = LicenseType.Trial;

    /// <summary>
    /// Número máximo de sucursales permitidas
    /// </summary>
    public int MaxBranches { get; set; } = 1;

    /// <summary>
    /// Número máximo de usuarios permitidos
    /// </summary>
    public int MaxUsers { get; set; } = 3;

    /// <summary>
    /// Fecha de emisión de la licencia
    /// </summary>
    public DateTime IssuedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Fecha de expiración (null = perpetua)
    /// </summary>
    public DateTime? ExpiresAt { get; set; }

    /// <summary>
    /// Última fecha de validación exitosa
    /// </summary>
    public DateTime? LastValidation { get; set; }

    /// <summary>
    /// Contador de validaciones realizadas
    /// </summary>
    public int ValidationCount { get; set; } = 0;
}