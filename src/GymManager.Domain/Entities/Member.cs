using GymManager.Domain.Common;
using GymManager.Domain.Enums;

namespace GymManager.Domain.Entities;

/// <summary>
/// Entidad de Miembro/Cliente del gimnasio
/// Contiene todos los campos de la tabla gym.members
/// </summary>
public class Member : AuditableEntity
{
    /// <summary>
    /// Identificador unico del miembro (PK)
    /// </summary>
    public Guid MemberId { get; set; }

    /// <summary>
    /// ID de la sucursal a la que pertenece (FK)
    /// </summary>
    public Guid BranchId { get; set; }

    /// <summary>
    /// Codigo unico del miembro (ej: MEM-20241211-001)
    /// </summary>
    public string MemberCode { get; set; } = string.Empty;

    #region Informacion Basica

    /// <summary>
    /// Nombre(s) del miembro
    /// </summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Apellido(s) del miembro
    /// </summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Correo electronico
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Telefono fijo
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// Telefono celular
    /// </summary>
    public string? MobilePhone { get; set; }

    #endregion

    #region Datos Personales

    /// <summary>
    /// Fecha de nacimiento
    /// </summary>
    public DateOnly? BirthDate { get; set; }

    /// <summary>
    /// Genero (M, F, O)
    /// </summary>
    public Gender? Gender { get; set; }

    /// <summary>
    /// Tipo de documento de identidad (INE, Pasaporte, etc.)
    /// </summary>
    public string? IdDocumentType { get; set; }

    /// <summary>
    /// Numero del documento de identidad
    /// </summary>
    public string? IdDocumentNumber { get; set; }

    /// <summary>
    /// Ruta de la foto del miembro
    /// </summary>
    public string? PhotoPath { get; set; }

    #endregion

    #region Direccion

    /// <summary>
    /// Direccion (calle y numero)
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

    #endregion

    #region Contacto de Emergencia

    /// <summary>
    /// Nombre del contacto de emergencia
    /// </summary>
    public string? EmergencyContactName { get; set; }

    /// <summary>
    /// Telefono del contacto de emergencia
    /// </summary>
    public string? EmergencyContactPhone { get; set; }

    /// <summary>
    /// Relacion con el contacto de emergencia
    /// </summary>
    public string? EmergencyContactRelationship { get; set; }

    #endregion

    #region Informacion Adicional

    /// <summary>
    /// Notas medicas (alergias, condiciones, etc.)
    /// </summary>
    public string? MedicalNotes { get; set; }

    /// <summary>
    /// Notas generales
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// ID del miembro que lo refirio (FK nullable)
    /// </summary>
    public Guid? ReferredByMemberId { get; set; }

    /// <summary>
    /// Fecha de registro/inscripcion
    /// </summary>
    public DateOnly RegistrationDate { get; set; } = DateOnly.FromDateTime(DateTime.Now);

    #endregion

    #region Propiedades de Navegacion

    /// <summary>
    /// Sucursal a la que pertenece
    /// </summary>
    public virtual Branch? Branch { get; set; }

    /// <summary>
    /// Miembro que lo refirio
    /// </summary>
    public virtual Member? ReferredByMember { get; set; }

    /// <summary>
    /// Miembros referidos por este miembro
    /// </summary>
    public virtual ICollection<Member> ReferredMembers { get; set; } = new List<Member>();

    /// <summary>
    /// Membresias del miembro
    /// </summary>
    public virtual ICollection<Membership> Memberships { get; set; } = new List<Membership>();

    /// <summary>
    /// Asistencias del miembro
    /// </summary>
    public virtual ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();

    /// <summary>
    /// Ventas realizadas al miembro
    /// </summary>
    public virtual ICollection<Sale> Sales { get; set; } = new List<Sale>();

    /// <summary>
    /// Pagos realizados por el miembro
    /// </summary>
    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    /// <summary>
    /// Inscripciones a clases
    /// </summary>
    public virtual ICollection<ClassEnrollment> ClassEnrollments { get; set; } = new List<ClassEnrollment>();

    #endregion

    #region Propiedades Calculadas

    /// <summary>
    /// Nombre completo del miembro
    /// </summary>
    public string FullName => $"{FirstName} {LastName}";

    /// <summary>
    /// Edad calculada
    /// </summary>
    public int? Age
    {
        get
        {
            if (!BirthDate.HasValue) return null;

            var today = DateOnly.FromDateTime(DateTime.Today);
            var age = today.Year - BirthDate.Value.Year;

            if (BirthDate.Value > today.AddYears(-age))
                age--;

            return age;
        }
    }

    #endregion

    #region Propiedades Obsoletas (para compatibilidad)

    /// <summary>
    /// OBSOLETO: Usar EmergencyContactName
    /// </summary>
    [Obsolete("Usar EmergencyContactName en su lugar")]
    public string? EmergencyContact
    {
        get => EmergencyContactName;
        set => EmergencyContactName = value;
    }

    /// <summary>
    /// OBSOLETO: Usar EmergencyContactPhone
    /// </summary>
    [Obsolete("Usar EmergencyContactPhone en su lugar")]
    public string? EmergencyPhone
    {
        get => EmergencyContactPhone;
        set => EmergencyContactPhone = value;
    }

    /// <summary>
    /// OBSOLETO: Usar PhotoPath
    /// </summary>
    [Obsolete("Usar PhotoPath en su lugar")]
    public string? PhotoBase64
    {
        get => PhotoPath;
        set => PhotoPath = value;
    }

    /// <summary>
    /// OBSOLETO: Codigo de barras (no se usa actualmente)
    /// </summary>
    [Obsolete("Esta propiedad no se usa")]
    public string? Barcode { get; set; }

    #endregion
}