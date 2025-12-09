using GymManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymManager.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuración de Entity Framework para la entidad MembershipPlan
/// </summary>
public class MembershipPlanConfiguration : IEntityTypeConfiguration<MembershipPlan>
{
    public void Configure(EntityTypeBuilder<MembershipPlan> builder)
    {
        // Tabla y Schema
        builder.ToTable("MembershipPlans", "gym");

        // Clave primaria
        builder.HasKey(mp => mp.PlanId);

        // Propiedades
        builder.Property(mp => mp.PlanId)
            .HasColumnName("PlanId")
            .IsRequired();

        builder.Property(mp => mp.BranchId)
            .HasColumnName("BranchId");

        builder.Property(mp => mp.Name)
            .HasColumnName("Name")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(mp => mp.Description)
            .HasColumnName("Description")
            .HasMaxLength(500);

        builder.Property(mp => mp.DurationDays)
            .HasColumnName("DurationDays")
            .IsRequired();

        builder.Property(mp => mp.Price)
            .HasColumnName("Price")
            .HasPrecision(10, 2)
            .IsRequired();

        builder.Property(mp => mp.FreezeDaysAllowed)
            .HasColumnName("FreezeDaysAllowed")
            .HasDefaultValue(0);

        builder.Property(mp => mp.ClassAccessLimit)
            .HasColumnName("ClassAccessLimit");

        builder.Property(mp => mp.IsAvailable)
            .HasColumnName("IsAvailable")
            .HasDefaultValue(true);

        // Campos de auditoría
        builder.Property(mp => mp.IsActive)
            .HasColumnName("IsActive")
            .HasDefaultValue(true);

        builder.Property(mp => mp.CreatedAt)
            .HasColumnName("CreatedAt")
            .IsRequired();

        builder.Property(mp => mp.UpdatedAt)
            .HasColumnName("UpdatedAt");

        builder.Property(mp => mp.DeletedAt)
            .HasColumnName("DeletedAt");

        // Índices
        builder.HasIndex(mp => mp.BranchId)
            .HasDatabaseName("IX_MembershipPlans_BranchId");

        // Relaciones
        builder.HasOne(mp => mp.Branch)
            .WithMany(b => b.MembershipPlans)
            .HasForeignKey(mp => mp.BranchId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}