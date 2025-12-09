using GymManager.Domain.Common;
using GymManager.Domain.Enums;

namespace GymManager.Domain.Entities;

/// <summary>
/// Entidad de Miembro - Clientes del gimnasio
/// </summary>
public class Member : AuditableEntity
{
    /// <summary>
    /// Identificador único del miembro
    /// </summary>
    public Guid MemberId { get; set; }

    /// <summary>
    /// ID de la sucursal donde se registró
    /// </summary>
    public Guid BranchId { get; set; }

    /// <summary>
    /// Código único del miembro (ej: MBR-00001)
    /// </summary>
    public string MemberCode { get; set; } = string.Empty;

    /// <summary>
    /// Nombre(s) del miembro
    /// </summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Apellidos del miembro
    /// </summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Correo electrónico
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Teléfono de contacto
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// Fecha de nacimiento
    /// </summary>
    public DateOnly? BirthDate { get; set; }

    /// <summary>
    /// Género del miembro
    /// </summary>
    public Gender? Gender { get; set; }

    /// <summary>
    /// Dirección del miembro
    /// </summary>
    public string? Address { get; set; }

    /// <summary>
    /// Nombre del contacto de emergencia
    /// </summary>
    public string? EmergencyContact { get; set; }

    /// <summary>
    /// Teléfono del contacto de emergencia
    /// </summary>
    public string? EmergencyPhone { get; set; }

    /// <summary>
    /// Foto del miembro en Base64
    /// </summary>
    public string? PhotoBase64 { get; set; }

    /// <summary>
    /// Código de barras o QR para acceso
    /// </summary>
    public string? Barcode { get; set; }

    /// <summary>
    /// Notas adicionales sobre el miembro
    /// </summary>
    public string? Notes { get; set; }

    // ═══════════════════════════════════════════════════════════
    // PROPIEDADES CALCULADAS
    // ═══════════════════════════════════════════════════════════

    /// <summary>
    /// Nombre completo del miembro
    /// </summary>
    public string FullName => $"{FirstName} {LastName}".Trim();

    // ═══════════════════════════════════════════════════════════
    // PROPIEDADES DE NAVEGACIÓN
    // ═══════════════════════════════════════════════════════════

    /// <summary>
    /// Sucursal donde está registrado
    /// </summary>
    public virtual Branch Branch { get; set; } = null!;

    /// <summary>
    /// Membresías del miembro (historial)
    /// </summary>
    public virtual ICollection<Membership> Memberships { get; set; } = new List<Membership>();

    /// <summary>
    /// Registros de asistencia
    /// </summary>
    public virtual ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();

    /// <summary>
    /// Pagos realizados
    /// </summary>
    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    /// <summary>
    /// Ventas realizadas al miembro
    /// </summary>
    public virtual ICollection<Sale> Sales { get; set; } = new List<Sale>();

    /// <summary>
    /// Inscripciones a clases
    /// </summary>
    public virtual ICollection<ClassEnrollment> ClassEnrollments { get; set; } = new List<ClassEnrollment>();
}