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
    /// <summary>
    /// Venta completada
    /// </summary>
    Completed = 0,

    /// <summary>
    /// Venta pendiente (ej: apartado)
    /// </summary>
    Pending = 1,

    /// <summary>
    /// Venta cancelada
    /// </summary>
    Cancelled = 2,

    /// <summary>
    /// Venta reembolsada
    /// </summary>
    Refunded = 3
}