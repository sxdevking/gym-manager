using GymManager.Domain.Common;
using GymManager.Domain.Entities;
using GymManager.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace GymManager.Infrastructure.Persistence;

/// <summary>
/// DbContext principal del sistema GymManager
/// Usa EFCore.NamingConventions para convertir automaticamente a snake_case
/// </summary>
public class GymDbContext : DbContext
{
    // ═══════════════════════════════════════════════════════════
    // REGISTRO ESTATICO DE ENUMS (debe ejecutarse UNA vez al inicio)
    // ═══════════════════════════════════════════════════════════
    static GymDbContext()
    {
        // Registrar los enums de PostgreSQL en Npgsql
        // Esto mapea los enums de C# a los tipos ENUM de PostgreSQL
        NpgsqlConnection.GlobalTypeMapper.MapEnum<LicenseType>("gym.license_type_enum");
        NpgsqlConnection.GlobalTypeMapper.MapEnum<MembershipStatus>("gym.membership_status_enum");
        NpgsqlConnection.GlobalTypeMapper.MapEnum<PaymentStatus>("gym.payment_status_enum");
        NpgsqlConnection.GlobalTypeMapper.MapEnum<SaleStatus>("gym.sale_status_enum");
        NpgsqlConnection.GlobalTypeMapper.MapEnum<EnrollmentStatus>("gym.enrollment_status_enum");
        NpgsqlConnection.GlobalTypeMapper.MapEnum<CheckInMethod>("gym.checkin_method_enum");
        NpgsqlConnection.GlobalTypeMapper.MapEnum<Gender>("gym.gender_enum");
    }

    public GymDbContext(DbContextOptions<GymDbContext> options) : base(options)
    {
    }

    // ═══════════════════════════════════════════════════════════
    // DbSets
    // ═══════════════════════════════════════════════════════════
    public DbSet<License> Licenses => Set<License>();
    public DbSet<Branch> Branches => Set<Branch>();
    public DbSet<BranchSettings> BranchSettings => Set<BranchSettings>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<User> Users => Set<User>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<Member> Members => Set<Member>();
    public DbSet<MembershipPlan> MembershipPlans => Set<MembershipPlan>();
    public DbSet<Membership> Memberships => Set<Membership>();
    public DbSet<Attendance> Attendances => Set<Attendance>();
    public DbSet<PaymentMethod> PaymentMethods => Set<PaymentMethod>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<ProductCategory> ProductCategories => Set<ProductCategory>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<InventoryStock> InventoryStocks => Set<InventoryStock>();
    public DbSet<Sale> Sales => Set<Sale>();
    public DbSet<SaleItem> SaleItems => Set<SaleItem>();
    public DbSet<ClassType> ClassTypes => Set<ClassType>();
    public DbSet<ClassSchedule> ClassSchedules => Set<ClassSchedule>();
    public DbSet<ClassEnrollment> ClassEnrollments => Set<ClassEnrollment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Schema por defecto
        modelBuilder.HasDefaultSchema("gym");

        // ═══════════════════════════════════════════════════════════
        // REGISTRAR ENUMS DE POSTGRESQL
        // ═══════════════════════════════════════════════════════════
        modelBuilder.HasPostgresEnum<LicenseType>("gym", "license_type_enum");
        modelBuilder.HasPostgresEnum<MembershipStatus>("gym", "membership_status_enum");
        modelBuilder.HasPostgresEnum<PaymentStatus>("gym", "payment_status_enum");
        modelBuilder.HasPostgresEnum<SaleStatus>("gym", "sale_status_enum");
        modelBuilder.HasPostgresEnum<EnrollmentStatus>("gym", "enrollment_status_enum");
        modelBuilder.HasPostgresEnum<CheckInMethod>("gym", "checkin_method_enum");
        modelBuilder.HasPostgresEnum<Gender>("gym", "gender_enum");

        // Aplicar todas las configuraciones del ensamblado
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(GymDbContext).Assembly);
    }

    /// <summary>
    /// Override de SaveChanges para auditoria automatica
    /// </summary>
    public override int SaveChanges()
    {
        UpdateAuditFields();
        return base.SaveChanges();
    }

    /// <summary>
    /// Override de SaveChangesAsync para auditoria automatica
    /// </summary>
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateAuditFields();
        return await base.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Actualiza los campos de auditoria automaticamente
    /// </summary>
    private void UpdateAuditFields()
    {
        var entries = ChangeTracker.Entries<AuditableEntity>();

        foreach (var entry in entries)
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    entry.Entity.IsActive = true;
                    break;

                case EntityState.Modified:
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    break;

                case EntityState.Deleted:
                    // Convertir a soft delete
                    entry.State = EntityState.Modified;
                    entry.Entity.DeletedAt = DateTime.UtcNow;
                    entry.Entity.IsActive = false;
                    break;
            }
        }
    }
}