using GymManager.Domain.Common;

namespace GymManager.Domain.Entities;

/// <summary>
/// Entidad de Sucursal - Representa cada ubicación física del gimnasio
/// </summary>
public class Branch : AuditableEntity
{
    /// <summary>
    /// Identificador único de la sucursal
    /// </summary>
    public Guid BranchId { get; set; }

    /// <summary>
    /// Código único de la sucursal (ej: GYM-001)
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Nombre de la sucursal
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Dirección física
    /// </summary>
    public string? Address { get; set; }

    /// <summary>
    /// Teléfono de contacto
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// Correo electrónico
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Hora de apertura
    /// </summary>
    public TimeOnly? OpenTime { get; set; }

    /// <summary>
    /// Hora de cierre
    /// </summary>
    public TimeOnly? CloseTime { get; set; }

    // ═══════════════════════════════════════════════════════════
    // PROPIEDADES DE NAVEGACIÓN (Relaciones con otras entidades)
    // ═══════════════════════════════════════════════════════════

    /// <summary>
    /// Configuración de marca de esta sucursal
    /// </summary>
    public virtual BranchSettings? Settings { get; set; }

    /// <summary>
    /// Usuarios asignados a esta sucursal
    /// </summary>
    public virtual ICollection<User> Users { get; set; } = new List<User>();

    /// <summary>
    /// Miembros registrados en esta sucursal
    /// </summary>
    public virtual ICollection<Member> Members { get; set; } = new List<Member>();

    /// <summary>
    /// Planes de membresía de esta sucursal
    /// </summary>
    public virtual ICollection<MembershipPlan> MembershipPlans { get; set; } = new List<MembershipPlan>();

    /// <summary>
    /// Inventario de productos en esta sucursal
    /// </summary>
    public virtual ICollection<InventoryStock> InventoryStocks { get; set; } = new List<InventoryStock>();

    /// <summary>
    /// Ventas realizadas en esta sucursal
    /// </summary>
    public virtual ICollection<Sale> Sales { get; set; } = new List<Sale>();

    /// <summary>
    /// Horarios de clases en esta sucursal
    /// </summary>
    public virtual ICollection<ClassSchedule> ClassSchedules { get; set; } = new List<ClassSchedule>();
}