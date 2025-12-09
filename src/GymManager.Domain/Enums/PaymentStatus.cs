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
    /// <summary>
    /// Pago completado exitosamente
    /// </summary>
    Completed = 0,

    /// <summary>
    /// Pago pendiente de procesar
    /// </summary>
    Pending = 1,

    /// <summary>
    /// Pago reembolsado al cliente
    /// </summary>
    Refunded = 2,

    /// <summary>
    /// Pago fallido o rechazado
    /// </summary>
    Failed = 3
}