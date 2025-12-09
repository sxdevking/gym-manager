using GymManager.Domain.Common;

namespace GymManager.Domain.Entities;

/// <summary>
/// Entidad de Método de Pago - Catálogo de formas de pago aceptadas
/// </summary>
public class PaymentMethod : AuditableEntity
{
    /// <summary>
    /// Identificador único del método de pago
    /// </summary>
    public Guid PaymentMethodId { get; set; }

    /// <summary>
    /// Nombre del método (ej: Efectivo, Tarjeta de Crédito, Transferencia)
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Descripción del método de pago
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Indica si requiere referencia (ej: número de transacción)
    /// </summary>
    public bool RequiresReference { get; set; } = false;

    // ═══════════════════════════════════════════════════════════
    // PROPIEDADES DE NAVEGACIÓN
    // ═══════════════════════════════════════════════════════════

    /// <summary>
    /// Pagos realizados con este método
    /// </summary>
    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    /// <summary>
    /// Ventas pagadas con este método
    /// </summary>
    public virtual ICollection<Sale> Sales { get; set; } = new List<Sale>();
}