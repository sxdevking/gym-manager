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
    /// <summary>
    /// Masculino
    /// </summary>
    Male = 0,

    /// <summary>
    /// Femenino
    /// </summary>
    Female = 1,

    /// <summary>
    /// Otro / Prefiere no decir
    /// </summary>
    Other = 2
}