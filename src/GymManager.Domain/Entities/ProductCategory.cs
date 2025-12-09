using GymManager.Domain.Common;

namespace GymManager.Domain.Entities;

/// <summary>
/// Entidad de Categoría de Producto - Clasificación de productos
/// </summary>
public class ProductCategory : AuditableEntity
{
    /// <summary>
    /// Identificador único de la categoría
    /// </summary>
    public Guid CategoryId { get; set; }

    /// <summary>
    /// Nombre de la categoría (ej: Bebidas, Suplementos, Accesorios)
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Descripción de la categoría
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Orden de visualización
    /// </summary>
    public int DisplayOrder { get; set; } = 0;

    // ═══════════════════════════════════════════════════════════
    // PROPIEDADES DE NAVEGACIÓN
    // ═══════════════════════════════════════════════════════════

    /// <summary>
    /// Productos en esta categoría
    /// </summary>
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}