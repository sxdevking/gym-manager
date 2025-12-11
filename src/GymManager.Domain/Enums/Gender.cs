using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManager.Domain.Enums;

/// <summary>
/// Género del miembro
/// </summary>
public enum Gender
{
    [PgName("M")]
    M = 0,

    [PgName("F")]
    F = 1,

    [PgName("O")]
    O = 2
}

