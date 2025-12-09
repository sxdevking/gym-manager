using GymManager.Domain.Common;
using GymManager.Domain.Enums;

namespace GymManager.Domain.Entities;

/// <summary>
/// Entidad de Membresía - Registro de membresías activas e históricas
/// </summary>
public class Membership : AuditableEntity
{
    /// <summary>
    /// Identificador único de la membresía
    /// </summary>
    public Guid MembershipId { get; set; }

    /// <summary>
    /// ID del miembro
    /// </summary>
    public Guid MemberId { get; set; }

    /// <summary>
    /// ID del plan contratado
    /// </summary>
    public Guid PlanId { get; set; }

    /// <summary>
    /// Fecha de inicio de la membresía
    /// </summary>
    public DateTime StartDate { get; set; }

    /// <summary>
    /// Fecha de fin de la membresía
    /// </summary>
    public DateTime EndDate { get; set; }

    /// <summary>
    /// Estado actual de la membresía
    /// </summary>
    public MembershipStatus Status { get; set; } = MembershipStatus.Active;

    /// <summary>
    /// Precio pagado (puede diferir del precio del plan por descuentos)
    /// </summary>
    public decimal PricePaid { get; set; }

    /// <summary>
    /// Días de congelamiento utilizados
    /// </summary>
    public int FreezeDaysUsed { get; set; } = 0;

    /// <summary>
    /// Fecha de inicio del congelamiento actual
    /// </summary>
    public DateTime? FreezeStartDate { get; set; }

    /// <summary>
    /// Notas sobre la membresía
    /// </summary>
    public string? Notes { get; set; }

    // ═══════════════════════════════════════════════════════════
    // PROPIEDADES CALCULADAS
    // ═══════════════════════════════════════════════════════════

    /// <summary>
    /// Indica si la membresía está vigente
    /// </summary>
    public bool IsCurrentlyActive =>
        Status == MembershipStatus.Active &&
        DateTime.UtcNow >= StartDate &&
        DateTime.UtcNow <= EndDate;

    /// <summary>
    /// Días restantes de la membresía
    /// </summary>
    public int DaysRemaining =>
        Status == MembershipStatus.Active
            ? Math.Max(0, (EndDate - DateTime.UtcNow).Days)
            : 0;

    // ═══════════════════════════════════════════════════════════
    // PROPIEDADES DE NAVEGACIÓN
    // ═══════════════════════════════════════════════════════════

    /// <summary>
    /// Miembro dueño de la membresía
    /// </summary>
    public virtual Member Member { get; set; } = null!;

    /// <summary>
    /// Plan contratado
    /// </summary>
    public virtual MembershipPlan Plan { get; set; } = null!;

    /// <summary>
    /// Pagos asociados a esta membresía
    /// </summary>
    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}