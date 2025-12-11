using GymManager.Domain.Common;
using GymManager.Domain.Enums;

namespace GymManager.Domain.Entities;

/// <summary>
/// Entidad de Asistencia - Registro de entradas y salidas al gimnasio
/// </summary>
public class Attendance : AuditableEntity
{
    /// <summary>
    /// Identificador único del registro
    /// </summary>
    public Guid AttendanceId { get; set; }

    /// <summary>
    /// ID del miembro
    /// </summary>
    public Guid MemberId { get; set; }

    /// <summary>
    /// ID de la sucursal donde se registró
    /// </summary>
    public Guid BranchId { get; set; }

    /// <summary>
    /// Fecha y hora de entrada
    /// </summary>
    public DateTime CheckInTime { get; set; }

    /// <summary>
    /// Fecha y hora de salida (null si aún está en el gimnasio)
    /// </summary>
    public DateTime? CheckOutTime { get; set; }

    /// <summary>
    /// Método utilizado para el check-in
    /// </summary>
    public CheckInMethod CheckInMethod { get; set; } = CheckInMethod.MANUAL;

    /// <summary>
    /// Duración de la visita en minutos (calculado automáticamente)
    /// </summary>
    public int? DurationMinutes { get; set; }

    /// <summary>
    /// Notas adicionales
    /// </summary>
    public string? Notes { get; set; }

    // ═══════════════════════════════════════════════════════════
    // PROPIEDADES CALCULADAS
    // ═══════════════════════════════════════════════════════════

    /// <summary>
    /// Indica si el miembro aún está en el gimnasio
    /// </summary>
    public bool IsCurrentlyInGym => CheckInTime != default && CheckOutTime == null;

    // ═══════════════════════════════════════════════════════════
    // PROPIEDADES DE NAVEGACIÓN
    // ═══════════════════════════════════════════════════════════

    /// <summary>
    /// Miembro que registró asistencia
    /// </summary>
    public virtual Member Member { get; set; } = null!;

    /// <summary>
    /// Sucursal donde se registró
    /// </summary>
    public virtual Branch Branch { get; set; } = null!;
}