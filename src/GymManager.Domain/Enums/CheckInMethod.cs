using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManager.Domain.Enums;

/// <summary>
/// Métodos de registro de entrada/salida
/// </summary>
public enum CheckInMethod
{
    /// <summary>
    /// Tarjeta de acceso
    /// </summary>
    Card = 0,

    /// <summary>
    /// Código QR
    /// </summary>
    QR = 1,

    /// <summary>
    /// Huella digital u otro biométrico
    /// </summary>
    Biometric = 2,

    /// <summary>
    /// Registro manual por staff
    /// </summary>
    Manual = 3
}