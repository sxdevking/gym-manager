using GymManager.Domain.Common;

namespace GymManager.Domain.Entities;

/// <summary>
/// Entidad de Configuración de Sucursal - Personalización de marca por sucursal
/// </summary>
public class BranchSettings : AuditableEntity
{
    /// <summary>
    /// Identificador único (mismo que BranchId - relación 1:1)
    /// </summary>
    public Guid BranchId { get; set; }

    /// <summary>
    /// Nombre comercial del gimnasio para esta sucursal
    /// </summary>
    public string? BusinessName { get; set; }

    /// <summary>
    /// Logo en formato Base64 o ruta al archivo
    /// </summary>
    public string? LogoBase64 { get; set; }

    /// <summary>
    /// Color primario de la marca (hex: #RRGGBB)
    /// </summary>
    public string PrimaryColor { get; set; } = "#3B82F6";

    /// <summary>
    /// Color secundario de la marca (hex: #RRGGBB)
    /// </summary>
    public string SecondaryColor { get; set; } = "#1E293B";

    /// <summary>
    /// Texto para pie de página en tickets/recibos
    /// </summary>
    public string? ReceiptFooter { get; set; }

    /// <summary>
    /// RFC o identificación fiscal
    /// </summary>
    public string? TaxId { get; set; }

    /// <summary>
    /// Información adicional para tickets
    /// </summary>
    public string? TicketInfo { get; set; }

    // ═══════════════════════════════════════════════════════════
    // PROPIEDADES DE NAVEGACIÓN
    // ═══════════════════════════════════════════════════════════

    /// <summary>
    /// Sucursal a la que pertenece esta configuración
    /// </summary>
    public virtual Branch Branch { get; set; } = null!;
}