using GymManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymManager.Infrastructure.Persistence.Configurations;

// ============================================================
// MEMBERSHIP PLANS (08_membership_plans.sql)
// ============================================================
public class MembershipPlanConfiguration : IEntityTypeConfiguration<MembershipPlan>
{
    public void Configure(EntityTypeBuilder<MembershipPlan> builder)
    {
        builder.ToTable("membershipplans");

        builder.HasKey(mp => mp.PlanId);

        // C#: Name -> SQL: plan_name
        builder.Property(mp => mp.Name)
            .HasColumnName("plan_name")
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(mp => mp.Description)
            .HasColumnType("text");

        builder.Property(mp => mp.DurationDays)
            .IsRequired();

        builder.Property(mp => mp.Price)
            .HasPrecision(10, 2);

        // C#: FreezeDaysAllowed -> SQL: max_freezing_days
        builder.Property(mp => mp.FreezeDaysAllowed)
            .HasColumnName("max_freezing_days");

        // C#: ClassAccessLimit -> SQL: max_classes_per_week
        builder.Property(mp => mp.ClassAccessLimit)
            .HasColumnName("max_classes_per_week");

        // C#: IsAvailable -> SQL: is_active
        builder.Property(mp => mp.IsAvailable)
            .HasColumnName("is_active");

        // Relaciones
        builder.HasOne(mp => mp.Branch)
            .WithMany(b => b.MembershipPlans)
            .HasForeignKey(mp => mp.BranchId)
            .OnDelete(DeleteBehavior.Cascade);

        // Ignorar propiedades de auditoria que no existen
        builder.Ignore(mp => mp.DeletedBy);
        builder.Ignore(mp => mp.IsDeleted);

        // Filtro soft delete
        builder.HasQueryFilter(mp => mp.DeletedAt == null);
    }
}