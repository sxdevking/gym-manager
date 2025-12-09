using GymManager.Domain.Common;

namespace GymManager.Domain.Entities;

/// <summary>
/// Entidad de Usuario - Usuarios que operan el sistema
/// </summary>
public class User : AuditableEntity
{
    /// <summary>
    /// Identificador único del usuario
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// ID de la sucursal donde trabaja el usuario
    /// </summary>
    public Guid BranchId { get; set; }

    /// <summary>
    /// Nombre de usuario para login (único)
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Correo electrónico (único)
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Hash de la contraseña
    /// </summary>
    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>
    /// Nombre completo del usuario
    /// </summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// Teléfono de contacto
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// Foto de perfil en Base64
    /// </summary>
    public string? AvatarBase64 { get; set; }

    /// <summary>
    /// Último inicio de sesión
    /// </summary>
    public DateTime? LastLoginAt { get; set; }

    /// <summary>
    /// Número de intentos de login fallidos
    /// </summary>
    public int FailedLoginAttempts { get; set; } = 0;

    /// <summary>
    /// Fecha hasta la cual está bloqueado el usuario
    /// </summary>
    public DateTime? LockoutUntil { get; set; }

    // ═══════════════════════════════════════════════════════════
    // PROPIEDADES DE NAVEGACIÓN
    // ═══════════════════════════════════════════════════════════

    /// <summary>
    /// Sucursal donde trabaja el usuario
    /// </summary>
    public virtual Branch Branch { get; set; } = null!;

    /// <summary>
    /// Roles asignados al usuario
    /// </summary>
    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();

    /// <summary>
    /// Ventas realizadas por este usuario
    /// </summary>
    public virtual ICollection<Sale> Sales { get; set; } = new List<Sale>();

    /// <summary>
    /// Pagos procesados por este usuario
    /// </summary>
    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}