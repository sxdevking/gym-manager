using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManager.Domain.Common;

/// <summary>
/// Interface para implementar borrado lógico (soft delete)
/// Las entidades que implementen esta interface no se eliminarán físicamente
/// </summary>
public interface ISoftDeletable
{
    /// <summary>
    /// Fecha y hora en que el registro fue eliminado (null si está activo)
    /// </summary>
    DateTime? DeletedAt { get; set; }

    /// <summary>
    /// Usuario que eliminó el registro
    /// </summary>
    string? DeletedBy { get; set; }
}
