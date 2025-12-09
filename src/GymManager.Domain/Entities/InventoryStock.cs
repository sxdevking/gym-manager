using GymManager.Domain.Common;

namespace GymManager.Domain.Entities;

/// <summary>
/// Entidad de Stock de Inventario - Control de existencias por sucursal
/// </summary>
public class InventoryStock : AuditableEntity
{
    /// <summary>
    /// Identificador único del registro de stock
    /// </summary>
    public Guid StockId { get; set; }

    /// <summary>
    /// ID del producto
    /// </summary>
    public Guid ProductId { get; set; }

    /// <summary>
    /// ID de la sucursal
    /// </summary>
    public Guid BranchId { get; set; }

    /// <summary>
    /// Cantidad disponible en stock
    /// </summary>
    public int Quantity { get; set; } = 0;

    /// <summary>
    /// Cantidad actualmente rentada (para productos rentables)
    /// </summary>
    public int RentedQuantity { get; set; } = 0;

    /// <summary>
    /// Última fecha de reabastecimiento
    /// </summary>
    public DateTime? LastRestockDate { get; set; }

    // ═══════════════════════════════════════════════════════════
    // PROPIEDADES CALCULADAS
    // ═══════════════════════════════════════════════════════════

    /// <summary>
    /// Stock total disponible (restando los rentados)
    /// </summary>
    public int AvailableQuantity => Quantity - RentedQuantity;

    // ═══════════════════════════════════════════════════════════
    // PROPIEDADES DE NAVEGACIÓN
    // ═══════════════════════════════════════════════════════════

    /// <summary>
    /// Producto
    /// </summary>
    public virtual Product Product { get; set; } = null!;

    /// <summary>
    /// Sucursal
    /// </summary>
    public virtual Branch Branch { get; set; } = null!;
}