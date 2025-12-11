using GymManager.Domain.Common;
using GymManager.Domain.Enums;

namespace GymManager.Domain.Entities;

/// <summary>
/// Entidad de Venta - Encabezado de ventas de productos
/// </summary>
public class Sale : AuditableEntity
{
    /// <summary>
    /// Identificador único de la venta
    /// </summary>
    public Guid SaleId { get; set; }

    /// <summary>
    /// ID de la sucursal donde se realizó la venta
    /// </summary>
    public Guid BranchId { get; set; }

    /// <summary>
    /// ID del miembro (null si es venta a público general)
    /// </summary>
    public Guid? MemberId { get; set; }

    /// <summary>
    /// ID del usuario que realizó la venta
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// ID del método de pago utilizado
    /// </summary>
    public Guid PaymentMethodId { get; set; }

    /// <summary>
    /// Número de ticket (ej: TKT-20241208-0001)
    /// </summary>
    public string TicketNumber { get; set; } = string.Empty;

    /// <summary>
    /// Fecha y hora de la venta
    /// </summary>
    public DateTime SaleDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Subtotal antes de descuentos
    /// </summary>
    public decimal Subtotal { get; set; }

    /// <summary>
    /// Monto de descuento aplicado
    /// </summary>
    public decimal DiscountAmount { get; set; } = 0;

    /// <summary>
    /// Monto de impuestos
    /// </summary>
    public decimal TaxAmount { get; set; } = 0;

    /// <summary>
    /// Total de la venta
    /// </summary>
    public decimal Total { get; set; }

    /// <summary>
    /// Estado de la venta
    /// </summary>
    public SaleStatus Status { get; set; } = SaleStatus.COMPLETED;

    /// <summary>
    /// Referencia de pago (número de transacción, etc.)
    /// </summary>
    public string? PaymentReference { get; set; }

    /// <summary>
    /// Notas adicionales
    /// </summary>
    public string? Notes { get; set; }

    // ═══════════════════════════════════════════════════════════
    // PROPIEDADES DE NAVEGACIÓN
    // ═══════════════════════════════════════════════════════════

    /// <summary>
    /// Sucursal donde se realizó la venta
    /// </summary>
    public virtual Branch Branch { get; set; } = null!;

    /// <summary>
    /// Miembro que realizó la compra (si aplica)
    /// </summary>
    public virtual Member? Member { get; set; }

    /// <summary>
    /// Usuario que procesó la venta
    /// </summary>
    public virtual User User { get; set; } = null!;

    /// <summary>
    /// Método de pago utilizado
    /// </summary>
    public virtual PaymentMethod PaymentMethod { get; set; } = null!;

    /// <summary>
    /// Detalle de productos vendidos
    /// </summary>
    public virtual ICollection<SaleItem> SaleItems { get; set; } = new List<SaleItem>();
}