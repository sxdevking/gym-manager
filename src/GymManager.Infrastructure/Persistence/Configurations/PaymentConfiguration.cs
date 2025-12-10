using GymManager.Domain.Entities;
using GymManager.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymManager.Infrastructure.Persistence.Configurations;

// ============================================================
// PAYMENTS (12_payments.sql)
// ============================================================
public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable("payments");

        builder.HasKey(p => p.PaymentId);

        builder.Property(p => p.Amount)
            .HasPrecision(10, 2)
            .IsRequired();

        builder.Property(p => p.PaymentDate)
            .IsRequired();

        // Enum Status
        builder.Property(p => p.Status)
            .HasConversion(
                v => v.ToString().ToUpper(),
                v => Enum.Parse<PaymentStatus>(v, true))
            .HasMaxLength(20);

        // C#: Reference -> SQL: reference_number
        builder.Property(p => p.Reference)
            .HasColumnName("reference_number")
            .HasMaxLength(100);

        builder.Property(p => p.Notes)
            .HasColumnType("text");

        // C#: ProcessedByUserId -> SQL: created_by (mapeo aproximado)
        builder.Property(p => p.ProcessedByUserId)
            .HasColumnName("created_by");

        // Indices
        builder.HasIndex(p => p.MembershipId);
        builder.HasIndex(p => p.MemberId);
        builder.HasIndex(p => p.PaymentDate);
        builder.HasIndex(p => p.Status);

        // Relaciones
        builder.HasOne(p => p.Membership)
            .WithMany(m => m.Payments)
            .HasForeignKey(p => p.MembershipId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(p => p.Member)
            .WithMany(m => m.Payments)
            .HasForeignKey(p => p.MemberId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(p => p.PaymentMethod)
            .WithMany(pm => pm.Payments)
            .HasForeignKey(p => p.PaymentMethodId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.ProcessedByUser)
            .WithMany(u => u.Payments)
            .HasForeignKey(p => p.ProcessedByUserId)
            .OnDelete(DeleteBehavior.SetNull);

        // Ignorar propiedades de auditoria
        builder.Ignore(p => p.DeletedBy);
        builder.Ignore(p => p.IsDeleted);
        builder.Ignore(p => p.DeletedAt);
        builder.Ignore(p => p.UpdatedAt);
        builder.Ignore(p => p.UpdatedBy);
        builder.Ignore(p => p.CreatedBy); // Ya mapeado a ProcessedByUserId
        builder.Ignore(p => p.CreatedAt); // payment_date es la fecha

        // Filtro soft delete
        builder.HasQueryFilter(p => p.DeletedAt == null);
    }
}
