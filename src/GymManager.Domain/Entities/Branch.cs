using GymManager.Domain.Common;

namespace GymManager.Domain.Entities;

/// <summary>
/// Entidad de Sucursal - Representa cada ubicación física del gimnasio
/// </summary>
public class Branch : AuditableEntity
{
    /// <summary>
    /// Identificador unico de la sucursal
    /// </summary>
    public Guid BranchId { get; set; }

    /// <summary>
    /// ID de la licencia a la que pertenece esta sucursal
    /// </summary>
    public Guid LicenseId { get; set; }

    /// <summary>
    /// Codigo unico de la sucursal (ej: GYM-001)
    /// </summary>
    public string BranchCode { get; set; } = string.Empty;

    /// <summary>
    /// Nombre de la sucursal
    /// </summary>
    public string BranchName { get; set; } = string.Empty;

    /// <summary>
    /// Direccion fisica
    /// </summary>
    public string? Address { get; set; }

    /// <summary>
    /// Ciudad
    /// </summary>
    public string? City { get; set; }

    /// <summary>
    /// Estado/Provincia
    /// </summary>
    public string? State { get; set; }

    /// <summary>
    /// Codigo postal
    /// </summary>
    public string? PostalCode { get; set; }

    /// <summary>
    /// Pais
    /// </summary>
    public string Country { get; set; } = "Mexico";

    /// <summary>
    /// Telefono de contacto
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// Correo electronico
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Indica si es la sucursal matriz/principal
    /// </summary>
    public bool IsHeadquarters { get; set; }

    // ═══════════════════════════════════════════════════════════
    // NOTA: opening_time y closing_time estan en BranchSettings
    // NO en esta tabla
    // ═══════════════════════════════════════════════════════════

    // ═══════════════════════════════════════════════════════════
    // PROPIEDADES DE NAVEGACION
    // ═══════════════════════════════════════════════════════════

    /// <summary>
    /// Licencia del software
    /// </summary>
    public virtual License License { get; set; } = null!;

    /// <summary>
    /// Configuracion de marca de esta sucursal (relacion 1:1)
    /// Los horarios de apertura/cierre estan aqui
    /// </summary>
    public virtual BranchSettings? Settings { get; set; }

    /// <summary>
    /// Usuarios asignados a esta sucursal
    /// </summary>
    public virtual ICollection<User> Users { get; set; } = new List<User>();

    /// <summary>
    /// Miembros registrados en esta sucursal
    /// </summary>
    public virtual ICollection<Member> Members { get; set; } = new List<Member>();

    /// <summary>
    /// Planes de membresia de esta sucursal
    /// </summary>
    public virtual ICollection<MembershipPlan> MembershipPlans { get; set; } = new List<MembershipPlan>();

    /// <summary>
    /// Inventario de productos en esta sucursal
    /// </summary>
    public virtual ICollection<InventoryStock> InventoryStocks { get; set; } = new List<InventoryStock>();

    /// <summary>
    /// Ventas realizadas en esta sucursal
    /// </summary>
    public virtual ICollection<Sale> Sales { get; set; } = new List<Sale>();

    /// <summary>
    /// Horarios de clases en esta sucursal
    /// </summary>
    public virtual ICollection<ClassSchedule> ClassSchedules { get; set; } = new List<ClassSchedule>();

    /// <summary>
    /// Membresias de esta sucursal
    /// </summary>
    public virtual ICollection<Membership> Memberships { get; set; } = new List<Membership>();

    /// <summary>
    /// Asistencias registradas en esta sucursal
    /// </summary>
    public virtual ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();

    /// <summary>
    /// Pagos realizados en esta sucursal
    /// </summary>
    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
