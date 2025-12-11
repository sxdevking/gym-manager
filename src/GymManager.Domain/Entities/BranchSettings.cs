using GymManager.Domain.Common;

namespace GymManager.Domain.Entities;

/// <summary>
/// Entidad de Configuración de Sucursal - Personalización de marca por sucursal
/// </summary>
public class BranchSettings : AuditableEntity
{
    /// <summary>
    /// Identificador unico de la configuracion
    /// </summary>
    public Guid SettingId { get; set; }

    /// <summary>
    /// ID de la sucursal (relacion 1:1)
    /// </summary>
    public Guid BranchId { get; set; }

    // ═══════════════════════════════════════════════════════════
    // COLORES DE MARCA
    // ═══════════════════════════════════════════════════════════

    /// <summary>
    /// Color primario de la marca (hex: #RRGGBB)
    /// </summary>
    public string PrimaryColor { get; set; } = "#1E40AF";

    /// <summary>
    /// Color secundario de la marca (hex: #RRGGBB)
    /// </summary>
    public string SecondaryColor { get; set; } = "#3B82F6";

    /// <summary>
    /// Color de acento (hex: #RRGGBB)
    /// </summary>
    public string AccentColor { get; set; } = "#10B981";

    // ═══════════════════════════════════════════════════════════
    // LOGOTIPOS
    // ═══════════════════════════════════════════════════════════

    /// <summary>
    /// Ruta al logo principal
    /// </summary>
    public string? LogoPath { get; set; }

    /// <summary>
    /// Ruta al logo pequeno
    /// </summary>
    public string? LogoSmallPath { get; set; }

    /// <summary>
    /// Ruta al favicon
    /// </summary>
    public string? FaviconPath { get; set; }

    // ═══════════════════════════════════════════════════════════
    // INFORMACION FISCAL
    // ═══════════════════════════════════════════════════════════

    /// <summary>
    /// Nombre comercial del gimnasio
    /// </summary>
    public string? BusinessName { get; set; }

    /// <summary>
    /// RFC o identificacion fiscal
    /// </summary>
    public string? TaxId { get; set; }

    // ═══════════════════════════════════════════════════════════
    // PERSONALIZACION DE RECIBOS
    // ═══════════════════════════════════════════════════════════

    /// <summary>
    /// Texto para encabezado en tickets/recibos
    /// </summary>
    public string? ReceiptHeader { get; set; }

    /// <summary>
    /// Texto para pie de pagina en tickets/recibos
    /// </summary>
    public string? ReceiptFooter { get; set; }

    // ═══════════════════════════════════════════════════════════
    // CONFIGURACION REGIONAL
    // ═══════════════════════════════════════════════════════════

    /// <summary>
    /// Zona horaria de la sucursal
    /// </summary>
    public string Timezone { get; set; } = "America/Mexico_City";

    /// <summary>
    /// Codigo de moneda (ISO 4217)
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

    // ═══════════════════════════════════════════════════════════
    // HORARIOS DE OPERACION
    // ═══════════════════════════════════════════════════════════

    /// <summary>
    /// Hora de apertura del gimnasio
    /// </summary>
    public TimeOnly OpeningTime { get; set; } = new TimeOnly(6, 0);

    /// <summary>
    /// Hora de cierre del gimnasio
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
