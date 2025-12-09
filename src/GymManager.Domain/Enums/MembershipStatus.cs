using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManager.Domain.Enums;

/// <summary>
/// Estados posibles de una membresía
/// </summary>
public enum MembershipStatus
{
    /// <summary>
    /// Membresía activa y vigente
    /// </summary>
    Active = 0,

    /// <summary>
    /// Membresía expirada (fecha fin alcanzada)
    /// </summary>
    Expired = 1,

    /// <summary>
    /// Membresía congelada temporalmente
    /// </summary>
    Frozen = 2,

    /// <summary>
    /// Membresía cancelada por el usuario o administrador
    /// </summary>
    Cancelled = 3
}
