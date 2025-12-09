using GymManager.Domain.Common;

namespace GymManager.Domain.Entities;

/// <summary>
/// Entidad de Tipo de Clase - Catálogo de clases grupales disponibles
/// </summary>
public class ClassType : AuditableEntity
{
    /// <summary>
    /// Identificador único del tipo de clase
    /// </summary>
    public Guid ClassTypeId { get; set; }

    /// <summary>
    /// Nombre de la clase (ej: Yoga, Spinning, Zumba)
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Descripción de la clase
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Duración estándar en minutos
    /// </summary>
    public int DurationMinutes { get; set; } = 60;

    /// <summary>
    /// Capacidad máxima de participantes
    /// </summary>
    public int MaxCapacity { get; set; } = 20;

    /// <summary>
    /// Color para mostrar en el calendario (hex: #RRGGBB)
    /// </summary>
    public string Color { get; set; } = "#3B82F6";

    /// <summary>
    /// Indica si la clase está activa
    /// </summary>
    public bool IsAvailable { get; set; } = true;

    // ═══════════════════════════════════════════════════════════
    // PROPIEDADES DE NAVEGACIÓN
    // ═══════════════════════════════════════════════════════════

    /// <summary>
    /// Horarios programados de este tipo de clase
    /// </summary>
    public virtual ICollection<ClassSchedule> ClassSchedules { get; set; } = new List<ClassSchedule>();
}