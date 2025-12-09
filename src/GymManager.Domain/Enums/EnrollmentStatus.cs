using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManager.Domain.Enums;

/// <summary>
/// Estados de inscripción a una clase
/// </summary>
public enum EnrollmentStatus
{
    /// <summary>
    /// Inscrito en la clase
    /// </summary>
    Enrolled = 0,

    /// <summary>
    /// Asistió a la clase
    /// </summary>
    Attended = 1,

    /// <summary>
    /// No se presentó a la clase
    /// </summary>
    NoShow = 2,

    /// <summary>
    /// Canceló su inscripción
    /// </summary>
    Cancelled = 3
}