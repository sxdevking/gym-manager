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
/// <summary>
/// Clase base para todas las entidades con campos de auditoria
/// Todas las entidades del sistema heredan de esta clase
/// </summary>
public abstract class AuditableEntity : ISoftDeletable
{
    /// <summary>
    /// Fecha y hora de creacion del registro (UTC)
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// ID del usuario que creo el registro (UUID en PostgreSQL)
    /// </summary>
    public Guid? CreatedBy { get; set; }

    /// <summary>
    /// Fecha y hora de ultima actualizacion (UTC)
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// ID del usuario que realizo la ultima actualizacion (UUID en PostgreSQL)
    /// </summary>
    public Guid? UpdatedBy { get; set; }

    /// <summary>
    /// Fecha de eliminacion logica (null = registro activo)
    /// </summary>
    public DateTime? DeletedAt { get; set; }

    /// <summary>
    /// ID del usuario que elimino el registro (UUID en PostgreSQL)
    /// NOTA: Esta columna no existe en todas las tablas de PostgreSQL
    /// </summary>
    public Guid? DeletedBy { get; set; }

    /// <summary>
    /// Indica si el registro esta activo
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Verifica si el registro ha sido eliminado logicamente
    /// </summary>
    public bool IsDeleted => DeletedAt.HasValue;
}