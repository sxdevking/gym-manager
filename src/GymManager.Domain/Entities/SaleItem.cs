using GymManager.Domain.Common;

namespace GymManager.Domain.Entities;

/// <summary>
/// Entidad de Detalle de Venta - Productos incluidos en una venta
/// </summary>
public class SaleItem : AuditableEntity
{
    /// <summary>
    /// Identificador único del item
    /// </summary>
    public Guid SaleItemId { get; set; }

    /// <summary>
    /// ID de la venta
    /// </summary>
    public Guid SaleId { get; set; }

    /// <summary>
    /// ID del producto
    /// </summary>
    public Guid ProductId { get; set; }

    /// <summary>
    /// Cantidad vendida
    /// </summary>
    public int Quantity { get; set; } = 1;

    /// <summary>
    /// Precio unitario al momento de la venta
    /// </summary>
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// Descuento aplicado a este item
    /// </summary>
    public decimal Discount { get; set; } = 0;

    /// <summary>
    /// Subtotal del item (Quantity * UnitPrice - Discount)
    /// </summary>
    public decimal Subtotal { get; set; }

    /// <summary>
    /// Indica si es una renta (para productos rentables)
    /// </summary>
    public bool IsRental { get; set; } = false;

    /// <summary>
    /// Fecha de devolución esperada (para rentas)
    /// </summary>
    public DateTime? RentalReturnDate { get; set; }

    /// <summary>
    /// Fecha de devolución real (para rentas)
    /// </summary>
    public DateTime? ActualReturnDate { get; set; }

    /// <summary>
    /// Depósito cobrado (para rentas)
    /// </summary>
    public decimal? DepositAmount { get; set; }

    /// <summary>
    /// Indica si el depósito fue devuelto
    /// </summary>
    public bool DepositReturned { get; set; } = false;

    // ═══════════════════════════════════════════════════════════
    // PROPIEDADES CALCULADAS
    // ═══════════════════════════════════════════════════════════

    /// <summary>
    /// Indica si la renta está pendiente de devolución
    /// </summary>
    public bool IsPendingReturn => IsRental && ActualReturnDate == null;

    // ═══════════════════════════════════════════════════════════
    // PROPIEDADES DE NAVEGACIÓN
    // ═══════════════════════════════════════════════════════════

    /// <summary>
    /// Venta a la que pertenece este item
    /// </summary>
    public virtual Sale Sale { get; set; } = null!;

    /// <summary>
    /// Producto vendido
    /// </summary>
    public virtual Product Product { get; set; } = null!;
}