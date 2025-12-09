using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManager.Domain.Enums;

/// <summary>
/// Tipos de licencia del software
/// </summary>
public enum LicenseType
{
    /// <summary>
    /// Licencia de prueba (30 días, 1 sucursal, 3 usuarios)
    /// </summary>
    Trial = 0,

    /// <summary>
    /// Licencia estándar (1 sucursal, 10 usuarios)
    /// </summary>
    Standard = 1,

    /// <summary>
    /// Licencia empresarial (sucursales y usuarios ilimitados)
    /// </summary>
    Enterprise = 2
}
