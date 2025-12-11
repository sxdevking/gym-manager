using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManager.Domain.Enums;

/// <summary>
/// Estados posibles de una membresía
/// </summary>
public enum MembershipStatus
{
    [PgName("ACTIVE")]
    ACTIVE = 0,

    [PgName("EXPIRED")]
    EXPIRED = 1,

    [PgName("FROZEN")]
    FROZEN = 2,

    [PgName("CANCELLED")]
    CANCELLED = 3
}
