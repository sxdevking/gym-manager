using GymManager.Domain.Common;

namespace GymManager.Domain.Entities;

/// <summary>
/// Entidad de Producto - Productos disponibles para venta
/// </summary>
public class Product : AuditableEntity
{
    /// <summary>
    /// Identificador único del producto
    /// </summary>
    public Guid ProductId { get; set; }

    /// <summary>
    /// ID de la categoría del producto
    /// </summary>
    public Guid CategoryId { get; set; }

    /// <summary>
    /// Código SKU del producto
    /// </summary>
    public string Sku { get; set; } = string.Empty;

    /// <summary>
    /// Nombre del producto
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Descripción del producto
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Precio de venta
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// Costo del producto (para cálculo de utilidad)
    /// </summary>
    public decimal? Cost { get; set; }

    /// <summary>
    /// Indica si es un producto rentable (ej: toallas)
    /// </summary>
    public bool IsRentable { get; set; } = false;

    /// <summary>
    /// Precio de renta (si aplica)
    /// </summary>
    public decimal? RentalPrice { get; set; }

    /// <summary>
    /// Depósito requerido para renta (si aplica)
    /// </summary>
    public decimal? RentalDeposit { get; set; }

    /// <summary>
    /// Stock mínimo para alertas
    /// </summary>
    public int MinStock { get; set; } = 5;

    /// <summary>
    /// Imagen del producto en Base64
    /// </summary>
    public string? ImageBase64 { get; set; }

    /// <summary>
    /// Indica si el producto está disponible para venta
    /// </summary>
    public bool IsAvailable { get; set; } = true;

    // ═══════════════════════════════════════════════════════════
    // PROPIEDADES DE NAVEGACIÓN
    // ═══════════════════════════════════════════════════════════

    /// <summary>
    /// Categoría del producto
    /// </summary>
    public virtual ProductCategory Category { get; set; } = null!;

    /// <summary>
    /// Stock en cada sucursal
    /// </summary>
    public virtual ICollection<InventoryStock> InventoryStocks { get; set; } = new List<InventoryStock>();

    /// <summary>
    /// Items de venta de este producto
    /// </summary>
    public virtual ICollection<SaleItem> SaleItems { get; set; } = new List<SaleItem>();
}