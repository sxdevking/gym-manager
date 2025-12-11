using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManager.Domain.Enums;

/// <summary>
/// Estados posibles de una venta
/// </summary>
public enum SaleStatus
{
    [PgName("COMPLETED")]
    COMPLETED = 0,

    [PgName("PENDING")]
    PENDING = 1,

    [PgName("CANCELLED")]
    CANCELLED = 2,

    [PgName("REFUNDED")]
    REFUNDED = 3
}
