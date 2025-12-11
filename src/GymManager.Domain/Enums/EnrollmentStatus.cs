using NpgsqlTypes;
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
    [PgName("ENROLLED")]
    ENROLLED = 0,

    [PgName("ATTENDED")]
    ATTENDED = 1,

    [PgName("NO_SHOW")]
    NO_SHOW = 2,

    [PgName("CANCELLED")]
    CANCELLED = 3
}
