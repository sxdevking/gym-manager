using GymManager.Domain.Common;

namespace GymManager.Domain.Entities;

/// <summary>
/// Entidad de Relación Usuario-Rol - Tabla intermedia para la relación muchos-a-muchos
/// </summary>
public class UserRole : AuditableEntity
{
    /// <summary>
    /// Identificador único de la relación
    /// </summary>
    public Guid UserRoleId { get; set; }

    /// <summary>
    /// ID del usuario
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// ID del rol
    /// </summary>
    public Guid RoleId { get; set; }

    /// <summary>
    /// Fecha de asignación del rol
    /// </summary>
    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;

    // ═══════════════════════════════════════════════════════════
    // PROPIEDADES DE NAVEGACIÓN
    // ═══════════════════════════════════════════════════════════

    /// <summary>
    /// Usuario al que se asigna el rol
    /// </summary>
    public virtual User User { get; set; } = null!;

    /// <summary>
    /// Rol asignado
    /// </summary>
    public virtual Role Role { get; set; } = null!;
}