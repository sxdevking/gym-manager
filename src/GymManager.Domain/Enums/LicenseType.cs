using NpgsqlTypes;
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
    /// Licencia de prueba (30 dias, 1 sucursal, 3 usuarios)
    /// </summary>
    [PgName("TRIAL")]
    TRIAL = 0,

    /// <summary>
    /// Licencia estandar (1 sucursal, 10 usuarios)
    /// </summary>
    [PgName("STANDARD")]
    STANDARD = 1,

    /// <summary>
    /// Licencia empresarial (sucursales y usuarios ilimitados)
    /// </summary>
    [PgName("ENTERPRISE")]
    ENTERPRISE = 2
}

