using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManager.Domain.Common;

/// <summary>
/// Interface para implementar borrado logico (soft delete)
/// Las entidades que implementen esta interface no se eliminaran fisicamente
/// </summary>
public interface ISoftDeletable
{
    /// <summary>
    /// Fecha y hora en que el registro fue eliminado (null si esta activo)
    /// </summary>
    DateTime? DeletedAt { get; set; }

    /// <summary>
    /// ID del usuario que elimino el registro (UUID en PostgreSQL)
    /// </summary>
    Guid? DeletedBy { get; set; }
}
