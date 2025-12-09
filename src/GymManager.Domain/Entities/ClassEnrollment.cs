using GymManager.Domain.Common;
using GymManager.Domain.Enums;

namespace GymManager.Domain.Entities;

/// <summary>
/// Entidad de Inscripción a Clase - Registro de miembros inscritos en clases
/// </summary>
public class ClassEnrollment : AuditableEntity
{
    /// <summary>
    /// Identificador único de la inscripción
    /// </summary>
    public Guid EnrollmentId { get; set; }

    /// <summary>
    /// ID del horario de clase
    /// </summary>
    public Guid ScheduleId { get; set; }

    /// <summary>
    /// ID del miembro inscrito
    /// </summary>
    public Guid MemberId { get; set; }

    /// <summary>
    /// Fecha específica de la clase (para una ocurrencia específica)
    /// </summary>
    public DateOnly ClassDate { get; set; }

    /// <summary>
    /// Fecha y hora de la inscripción
    /// </summary>
    public DateTime EnrolledAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Estado de la inscripción
    /// </summary>
    public EnrollmentStatus Status { get; set; } = EnrollmentStatus.Enrolled;

    /// <summary>
    /// Fecha y hora de check-in a la clase
    /// </summary>
    public DateTime? CheckedInAt { get; set; }

    /// <summary>
    /// Notas adicionales
    /// </summary>
    public string? Notes { get; set; }

    // ═══════════════════════════════════════════════════════════
    // PROPIEDADES DE NAVEGACIÓN
    // ═══════════════════════════════════════════════════════════

    /// <summary>
    /// Horario de clase
    /// </summary>
    public virtual ClassSchedule ClassSchedule { get; set; } = null!;

    /// <summary>
    /// Miembro inscrito
    /// </summary>
    public virtual Member Member { get; set; } = null!;
}