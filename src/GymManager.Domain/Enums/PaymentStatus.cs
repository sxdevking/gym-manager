using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManager.Domain.Enums;

/// <summary>
/// Estados posibles de un pago
/// </summary>
public enum PaymentStatus
{
    [PgName("COMPLETED")]
    COMPLETED = 0,

    [PgName("PENDING")]
    PENDING = 1,

    [PgName("REFUNDED")]
    REFUNDED = 2,

    [PgName("FAILED")]
    FAILED = 3
}
