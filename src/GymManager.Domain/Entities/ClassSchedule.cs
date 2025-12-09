using GymManager.Domain.Common;

namespace GymManager.Domain.Entities;

/// <summary>
/// Entidad de Horario de Clase - Programación de clases grupales
/// </summary>
public class ClassSchedule : AuditableEntity
{
    /// <summary>
    /// Identificador único del horario
    /// </summary>
    public Guid ScheduleId { get; set; }

    /// <summary>
    /// ID del tipo de clase
    /// </summary>
    public Guid ClassTypeId { get; set; }

    /// <summary>
    /// ID de la sucursal
    /// </summary>
    public Guid BranchId { get; set; }

    /// <summary>
    /// ID del instructor (usuario con rol Trainer)
    /// </summary>
    public Guid? InstructorId { get; set; }

    /// <summary>
    /// Día de la semana (0=Domingo, 1=Lunes, ..., 6=Sábado)
    /// </summary>
    public int DayOfWeek { get; set; }

    /// <summary>
    /// Hora de inicio
    /// </summary>
    public TimeOnly StartTime { get; set; }

    /// <summary>
    /// Hora de fin
    /// </summary>
    public TimeOnly EndTime { get; set; }

    /// <summary>
    /// Capacidad máxima (puede ser diferente al tipo de clase)
    /// </summary>
    public int MaxCapacity { get; set; }

    /// <summary>
    /// Lugar o sala donde se imparte
    /// </summary>
    public string? Room { get; set; }

    /// <summary>
    /// Indica si el horario está activo
    /// </summary>
    public bool IsAvailable { get; set; } = true;

    // ═══════════════════════════════════════════════════════════
    // PROPIEDADES CALCULADAS
    // ═══════════════════════════════════════════════════════════

    /// <summary>
    /// Nombre del día de la semana
    /// </summary>
    public string DayName => DayOfWeek switch
    {
        0 => "Domingo",
        1 => "Lunes",
        2 => "Martes",
        3 => "Miércoles",
        4 => "Jueves",
        5 => "Viernes",
        6 => "Sábado",
        _ => "Desconocido"
    };

    // ═══════════════════════════════════════════════════════════
    // PROPIEDADES DE NAVEGACIÓN
    // ═══════════════════════════════════════════════════════════

    /// <summary>
    /// Tipo de clase
    /// </summary>
    public virtual ClassType ClassType { get; set; } = null!;

    /// <summary>
    /// Sucursal donde se imparte
    /// </summary>
    public virtual Branch Branch { get; set; } = null!;

    /// <summary>
    /// Instructor asignado
    /// </summary>
    public virtual User? Instructor { get; set; }

    /// <summary>
    /// Inscripciones a este horario
    /// </summary>
    public virtual ICollection<ClassEnrollment> ClassEnrollments { get; set; } = new List<ClassEnrollment>();
}