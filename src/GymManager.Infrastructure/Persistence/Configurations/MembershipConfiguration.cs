using GymManager.Domain.Entities;
using GymManager.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymManager.Infrastructure.Persistence.Configurations;

// ============================================================
// MEMBERSHIPS (09_memberships.sql)
// ============================================================
public class MembershipConfiguration : IEntityTypeConfiguration<Membership>
{
    public void Configure(EntityTypeBuilder<Membership> builder)
    {
        builder.ToTable("memberships");

        builder.HasKey(m => m.MembershipId);

        builder.Property(m => m.MembershipId)
            .HasColumnName("membership_id");

        builder.Property(m => m.MemberId)
            .HasColumnName("member_id")
            .IsRequired();

        builder.Property(m => m.PlanId)
            .HasColumnName("plan_id")
            .IsRequired();

        builder.Property(m => m.StartDate)
            .HasColumnName("start_date")
            .IsRequired();

        builder.Property(m => m.EndDate)
            .HasColumnName("end_date")
            .IsRequired();

        // ENUM NATIVO DE POSTGRESQL - NO usar HasConversion
        builder.Property(m => m.Status)
            .HasColumnName("status");

        builder.Property(m => m.PricePaid)
            .HasColumnName("price_paid")
            .HasPrecision(10, 2);

        builder.Property(m => m.FreezeDaysUsed)
            .HasColumnName("frozen_days_used");

        builder.Property(m => m.FreezeStartDate)
            .HasColumnName("frozen_at");

        builder.Property(m => m.Notes)
            .HasColumnName("notes")
            .HasColumnType("text");

        // Auditoria
        builder.Property(m => m.IsActive)
            .HasColumnName("is_active");

        builder.Property(m => m.CreatedAt)
            .HasColumnName("created_at");

        builder.Property(m => m.CreatedBy)
            .HasColumnName("created_by");

        builder.Property(m => m.UpdatedAt)
            .HasColumnName("updated_at");

        builder.Property(m => m.UpdatedBy)
            .HasColumnName("updated_by");

        // Ignorar
        builder.Ignore(m => m.DeletedBy);
        builder.Ignore(m => m.IsDeleted);
        builder.Ignore(m => m.DeletedAt);
        builder.Ignore(m => m.IsCurrentlyActive);
        builder.Ignore(m => m.DaysRemaining);

        // Indices
        builder.HasIndex(m => m.MemberId);
        builder.HasIndex(m => m.PlanId);
        builder.HasIndex(m => m.Status);
        builder.HasIndex(m => new { m.StartDate, m.EndDate });

        // Relaciones
        builder.HasOne(m => m.Member)
            .WithMany(mb => mb.Memberships)
            .HasForeignKey(m => m.MemberId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(m => m.Plan)
            .WithMany(mp => mp.Memberships)
            .HasForeignKey(m => m.PlanId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
