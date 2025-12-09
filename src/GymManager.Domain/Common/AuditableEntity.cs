using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManager.Domain.Common;

/// <summary>
/// Clase base para todas las entidades con campos de auditoría
/// Todas las entidades del sistema heredan de esta clase
/// </summary>
public abstract class AuditableEntity : ISoftDeletable
{
    /// <summary>
    /// Fecha y hora de creación del registro (UTC)
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Usuario que creó el registro
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// Fecha y hora de última actualización (UTC)
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Usuario que realizó la última actualización
    /// </summary>
    public string? UpdatedBy { get; set; }

    /// <summary>
    /// Fecha de eliminación lógica (null = registro activo)
    /// </summary>
    public DateTime? DeletedAt { get; set; }

    /// <summary>
    /// Usuario que eliminó el registro
    /// </summary>
    public string? DeletedBy { get; set; }

    /// <summary>
    /// Indica si el registro está activo
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Verifica si el registro ha sido eliminado lógicamente
    /// </summary>
    public bool IsDeleted => DeletedAt.HasValue;
}
