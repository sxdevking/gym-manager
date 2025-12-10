using GymManager.Domain.Common;

namespace GymManager.Domain.Entities;

/// <summary>
/// Entidad de Configuración de Sucursal - Personalización de marca por sucursal
/// </summary>
public class BranchSettings : AuditableEntity
{
    /// <summary>
    /// Identificador unico (mismo que BranchId - relacion 1:1)
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
    /// Color de acento (hex: #RRGGBB)
    /// </summary>
    public string AccentColor { get; set; } = "#10B981";

    /// <summary>
    /// Ruta al logo pequeno
    /// </summary>
    public string? LogoSmallPath { get; set; }

    /// <summary>
    /// Ruta al favicon
    /// </summary>
    public string? FaviconPath { get; set; }

    /// <summary>
    /// Texto para encabezado en tickets/recibos
    /// </summary>
    public string? ReceiptHeader { get; set; }

    /// <summary>
    /// Texto para pie de pagina en tickets/recibos
    /// </summary>
    public string? ReceiptFooter { get; set; }

    /// <summary>
    /// RFC o identificacion fiscal
    /// </summary>
    public string? TaxId { get; set; }

    /// <summary>
    /// Informacion adicional para tickets
    /// </summary>
    public string? TicketInfo { get; set; }

    /// <summary>
    /// Zona horaria de la sucursal
    /// </summary>
    public string Timezone { get; set; } = "America/Mexico_City";

    /// <summary>
    /// Codigo de moneda
    /// </summary>
    public string CurrencyCode { get; set; } = "MXN";

    /// <summary>
    /// Simbolo de moneda
    /// </summary>
    public string CurrencySymbol { get; set; } = "$";

    /// <summary>
    /// Formato de fecha
    /// </summary>
    public string DateFormat { get; set; } = "dd/MM/yyyy";

    /// <summary>
    /// Formato de hora
    /// </summary>
    public string TimeFormat { get; set; } = "HH:mm";

    /// <summary>
    /// Hora de apertura
    /// </summary>
    public TimeOnly OpeningTime { get; set; } = new TimeOnly(6, 0);

    /// <summary>
    /// Hora de cierre
    /// </summary>
    public TimeOnly ClosingTime { get; set; } = new TimeOnly(22, 0);

    // ═══════════════════════════════════════════════════════════
    // PROPIEDADES DE NAVEGACION
    // ═══════════════════════════════════════════════════════════

    /// <summary>
    /// Sucursal a la que pertenece esta configuracion
    /// </summary>
    public virtual Branch Branch { get; set; } = null!;
}
