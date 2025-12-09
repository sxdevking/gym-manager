using GymManager.Domain.Common;

namespace GymManager.Domain.Entities;

/// <summary>
/// Entidad de Rol - Catálogo de roles del sistema
/// </summary>
public class Role : AuditableEntity
{
    /// <summary>
    /// Identificador único del rol
    /// </summary>
    public Guid RoleId { get; set; }

    /// <summary>
    /// Nombre del rol (ej: Administrator, Staff, Trainer)
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Descripción del rol y sus permisos
    /// </summary>
    public string? Description { get; set; }

    // ═══════════════════════════════════════════════════════════
    // PROPIEDADES DE NAVEGACIÓN
    // ═══════════════════════════════════════════════════════════

    /// <summary>
    /// Relaciones usuario-rol
    /// </summary>
    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}