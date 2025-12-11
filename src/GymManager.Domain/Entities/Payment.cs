using GymManager.Domain.Common;
using GymManager.Domain.Enums;

namespace GymManager.Domain.Entities;

/// <summary>
/// Entidad de Pago - Registro de pagos de membresías
/// </summary>
public class Payment : AuditableEntity
{
    /// <summary>
    /// Identificador único del pago
    /// </summary>
    public Guid PaymentId { get; set; }

    /// <summary>
    /// ID de la membresía pagada
    /// </summary>
    public Guid MembershipId { get; set; }

    /// <summary>
    /// ID del miembro que paga
    /// </summary>
    public Guid MemberId { get; set; }

    /// <summary>
    /// ID del método de pago utilizado
    /// </summary>
    public Guid PaymentMethodId { get; set; }

    /// <summary>
    /// ID del usuario que procesó el pago
    /// </summary>
    public Guid? ProcessedByUserId { get; set; }

    /// <summary>
    /// Monto del pago
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// Fecha y hora del pago
    /// </summary>
    public DateTime PaymentDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Estado del pago
    /// </summary>
    public PaymentStatus Status { get; set; } = PaymentStatus.COMPLETED;

    /// <summary>
    /// Referencia externa (número de transacción, autorización, etc.)
    /// </summary>
    public string? Reference { get; set; }

    /// <summary>
    /// Notas adicionales
    /// </summary>
    public string? Notes { get; set; }

    // ═══════════════════════════════════════════════════════════
    // PROPIEDADES DE NAVEGACIÓN
    // ═══════════════════════════════════════════════════════════

    /// <summary>
    /// Membresía pagada
    /// </summary>
    public virtual Membership Membership { get; set; } = null!;

    /// <summary>
    /// Miembro que realizó el pago
    /// </summary>
    public virtual Member Member { get; set; } = null!;

    /// <summary>
    /// Método de pago utilizado
    /// </summary>
    public virtual PaymentMethod PaymentMethod { get; set; } = null!;

    /// <summary>
    /// Usuario que procesó el pago
    /// </summary>
    public virtual User? ProcessedByUser { get; set; }
}