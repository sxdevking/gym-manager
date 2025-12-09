using System.Reflection;
using GymManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GymManager.Infrastructure.Persistence;

/// <summary>
/// Contexto de base de datos para Entity Framework Core
/// Conecta las entidades C# con las tablas de PostgreSQL
/// </summary>
public class GymDbContext : DbContext
{
    public GymDbContext(DbContextOptions<GymDbContext> options) : base(options)
    {
    }

    // ═══════════════════════════════════════════════════════════
    // DbSets - Cada uno representa una tabla en la base de datos
    // ═══════════════════════════════════════════════════════════

    // Licenciamiento
    public DbSet<License> Licenses => Set<License>();

    // Sucursales
    public DbSet<Branch> Branches => Set<Branch>();
    public DbSet<BranchSettings> BranchSettings => Set<BranchSettings>();

    // Usuarios y Roles
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<User> Users => Set<User>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();

    // Miembros y Membresías
    public DbSet<Member> Members => Set<Member>();
    public DbSet<MembershipPlan> MembershipPlans => Set<MembershipPlan>();
    public DbSet<Membership> Memberships => Set<Membership>();
    public DbSet<Attendance> Attendances => Set<Attendance>();

    // Pagos
    public DbSet<PaymentMethod> PaymentMethods => Set<PaymentMethod>();
    public DbSet<Payment> Payments => Set<Payment>();

    // Productos e Inventario
    public DbSet<ProductCategory> ProductCategories => Set<ProductCategory>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<InventoryStock> InventoryStocks => Set<InventoryStock>();

    // Ventas
    public DbSet<Sale> Sales => Set<Sale>();
    public DbSet<SaleItem> SaleItems => Set<SaleItem>();

    // Clases
    public DbSet<ClassType> ClassTypes => Set<ClassType>();
    public DbSet<ClassSchedule> ClassSchedules => Set<ClassSchedule>();
    public DbSet<ClassEnrollment> ClassEnrollments => Set<ClassEnrollment>();

    /// <summary>
    /// Configuración del modelo de datos
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Usar el schema 'gym' como está definido en PostgreSQL
        modelBuilder.HasDefaultSchema("gym");

        // Aplicar TODAS las configuraciones del ensamblado automáticamente
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        // ═══════════════════════════════════════════════════════════
        // CONVERTIR TODOS LOS NOMBRES A MINÚSCULAS (PostgreSQL)
        // ═══════════════════════════════════════════════════════════
        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            // Convertir nombre de tabla a minúsculas
            var tableName = entity.GetTableName();
            if (!string.IsNullOrEmpty(tableName))
            {
                entity.SetTableName(tableName.ToLowerInvariant());
            }

            // Convertir nombres de columnas a minúsculas
            foreach (var property in entity.GetProperties())
            {
                var columnName = property.GetColumnName();
                if (!string.IsNullOrEmpty(columnName))
                {
                    property.SetColumnName(columnName.ToLowerInvariant());
                }
            }

            // Convertir nombres de claves foráneas a minúsculas
            foreach (var key in entity.GetKeys())
            {
                var keyName = key.GetName();
                if (!string.IsNullOrEmpty(keyName))
                {
                    key.SetName(keyName.ToLowerInvariant());
                }
            }

            // Convertir nombres de índices a minúsculas
            foreach (var index in entity.GetIndexes())
            {
                var indexName = index.GetDatabaseName();
                if (!string.IsNullOrEmpty(indexName))
                {
                    index.SetDatabaseName(indexName.ToLowerInvariant());
                }
            }

            // Convertir nombres de foreign keys a minúsculas
            foreach (var foreignKey in entity.GetForeignKeys())
            {
                var fkName = foreignKey.GetConstraintName();
                if (!string.IsNullOrEmpty(fkName))
                {
                    foreignKey.SetConstraintName(fkName.ToLowerInvariant());
                }
            }
        }
    }

    /// <summary>
    /// Configuración adicional de opciones del contexto
    /// </summary>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        // Habilitar logging de queries sensibles solo en desarrollo
#if DEBUG
        optionsBuilder.EnableSensitiveDataLogging();
        optionsBuilder.EnableDetailedErrors();
#endif
    }

    /// <summary>
    /// Sobrescribir SaveChanges para manejar auditoría automáticamente
    /// </summary>
    public override int SaveChanges()
    {
        HandleAuditableEntities();
        return base.SaveChanges();
    }

    /// <summary>
    /// Sobrescribir SaveChangesAsync para manejar auditoría automáticamente
    /// </summary>
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        HandleAuditableEntities();
        return base.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Maneja automáticamente los campos de auditoría (CreatedAt, UpdatedAt)
    /// </summary>
    private void HandleAuditableEntities()
    {
        var entries = ChangeTracker.Entries()
            .Where(e => e.Entity is Domain.Common.AuditableEntity &&
                        (e.State == EntityState.Added || e.State == EntityState.Modified));

        foreach (var entry in entries)
        {
            var entity = (Domain.Common.AuditableEntity)entry.Entity;

            if (entry.State == EntityState.Added)
            {
                entity.CreatedAt = DateTime.UtcNow;
            }
            else
            {
                entity.UpdatedAt = DateTime.UtcNow;
            }
        }
    }
}