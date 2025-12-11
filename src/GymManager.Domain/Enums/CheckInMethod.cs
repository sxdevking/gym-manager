using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NpgsqlTypes;

namespace GymManager.Domain.Enums;

/// <summary>
/// Métodos de registro de entrada/salida
/// </summary>
public enum CheckInMethod
{
    [PgName("CARD")]
    CARD = 0,

    [PgName("QR")]
    QR = 1,

    [PgName("BIOMETRIC")]
    BIOMETRIC = 2,

    [PgName("MANUAL")]
    MANUAL = 3
}
