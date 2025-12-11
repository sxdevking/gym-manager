using GymManager.Domain.Common;
using GymManager.Domain.Enums;

namespace GymManager.Domain.Entities;

/// <summary>
/// Entidad de Licencia - Controla el licenciamiento del software por dongle USB
/// </summary>
public class License : AuditableEntity
{
    /// <summary>
    /// Identificador unico de la licencia
    /// </summary>
    public Guid LicenseId { get; set; }

    /// <summary>
    /// Clave de licencia cifrada
    /// </summary>
    public string LicenseKey { get; set; } = string.Empty;

    /// <summary>
    /// ID unico del hardware del dongle (hash SHA-256)
    /// </summary>
    public string HardwareId { get; set; } = string.Empty;

    /// <summary>
    /// Tipo de licencia (Trial, Standard, Enterprise)
    /// </summary>
    public LicenseType LicenseType { get; set; } = LicenseType.TRIAL;

    /// <summary>
    /// Numero maximo de sucursales permitidas
    /// </summary>
    public int MaxBranches { get; set; } = 1;

    /// <summary>
    /// Numero maximo de usuarios permitidos
    /// </summary>
    public int MaxUsers { get; set; } = 5;

    /// <summary>
    /// Fecha de emision de la licencia
    /// </summary>
    public DateTime IssuedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Fecha de expiracion (null = perpetua)
    /// </summary>
    public DateTime? ExpiresAt { get; set; }

    /// <summary>
    /// Ultima fecha de validacion exitosa
    /// </summary>
    public DateTime? LastValidation { get; set; }

    /// <summary>
    /// Contador de validaciones realizadas
    /// </summary>
    public int ValidationCount { get; set; } = 0;

    // ═══════════════════════════════════════════════════════════
    // PROPIEDADES DE NAVEGACION
    // ═══════════════════════════════════════════════════════════

    /// <summary>
    /// Sucursales asociadas a esta licencia
    /// </summary>
    public virtual ICollection<Branch> Branches { get; set; } = new List<Branch>();
}
