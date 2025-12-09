using GymManager.Domain.Common;

namespace GymManager.Domain.Entities;

/// <summary>
/// Entidad de Plan de Membresía - Planes disponibles para contratar
/// </summary>
public class MembershipPlan : AuditableEntity
{
    /// <summary>
    /// Identificador único del plan
    /// </summary>
    public Guid PlanId { get; set; }

    /// <summary>
    /// ID de la sucursal (null = disponible en todas)
    /// </summary>
    public Guid? BranchId { get; set; }

    /// <summary>
    /// Nombre del plan (ej: Mensual Básico, Anual Premium)
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Descripción detallada del plan
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Duración en días del plan
    /// </summary>
    public int DurationDays { get; set; }

    /// <summary>
    /// Precio del plan
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// Días de congelamiento permitidos
    /// </summary>
    public int FreezeDaysAllowed { get; set; } = 0;

    /// <summary>
    /// Cantidad de accesos a clases grupales incluidos (null = ilimitado)
    /// </summary>
    public int? ClassAccessLimit { get; set; }

    /// <summary>
    /// Indica si el plan está disponible para venta
    /// </summary>
    public bool IsAvailable { get; set; } = true;

    // ═══════════════════════════════════════════════════════════
    // PROPIEDADES DE NAVEGACIÓN
    // ═══════════════════════════════════════════════════════════

    /// <summary>
    /// Sucursal donde aplica el plan (null = todas)
    /// </summary>
    public virtual Branch? Branch { get; set; }

    /// <summary>
    /// Membresías que usan este plan
    /// </summary>
    public virtual ICollection<Membership> Memberships { get; set; } = new List<Membership>();
}