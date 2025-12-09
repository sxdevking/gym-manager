using GymManager.Domain.Entities;
using GymManager.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymManager.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuración de Entity Framework para la entidad Payment
/// </summary>
public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        // Tabla y Schema
        builder.ToTable("Payments", "gym");

        // Clave primaria
        builder.HasKey(p => p.PaymentId);

        // Propiedades
        builder.Property(p => p.PaymentId)
            .HasColumnName("PaymentId")
            .IsRequired();

        builder.Property(p => p.MembershipId)
            .HasColumnName("MembershipId")
            .IsRequired();

        builder.Property(p => p.MemberId)
            .HasColumnName("MemberId")
            .IsRequired();

        builder.Property(p => p.PaymentMethodId)
            .HasColumnName("PaymentMethodId")
            .IsRequired();

        builder.Property(p => p.ProcessedByUserId)
            .HasColumnName("ProcessedByUserId");

        builder.Property(p => p.Amount)
            .HasColumnName("Amount")
            .HasPrecision(10, 2)
            .IsRequired();

        builder.Property(p => p.PaymentDate)
            .HasColumnName("PaymentDate")
            .IsRequired();

        builder.Property(p => p.Status)
            .HasColumnName("Status")
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(p => p.Reference)
            .HasColumnName("Reference")
            .HasMaxLength(100);

        builder.Property(p => p.Notes)
            .HasColumnName("Notes");

        // Campos de auditoría
        builder.Property(p => p.IsActive)
            .HasColumnName("IsActive")
            .HasDefaultValue(true);

        builder.Property(p => p.CreatedAt)
            .HasColumnName("CreatedAt")
            .IsRequired();

        builder.Property(p => p.UpdatedAt)
            .HasColumnName("UpdatedAt");

        builder.Property(p => p.DeletedAt)
            .HasColumnName("DeletedAt");

        // Índices
        builder.HasIndex(p => p.MembershipId)
            .HasDatabaseName("IX_Payments_MembershipId");

        builder.HasIndex(p => p.MemberId)
            .HasDatabaseName("IX_Payments_MemberId");

        builder.HasIndex(p => p.PaymentDate)
            .HasDatabaseName("IX_Payments_PaymentDate");

        // Relaciones
        builder.HasOne(p => p.Membership)
            .WithMany(m => m.Payments)
            .HasForeignKey(p => p.MembershipId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Member)
            .WithMany(m => m.Payments)
            .HasForeignKey(p => p.MemberId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.PaymentMethod)
            .WithMany(pm => pm.Payments)
            .HasForeignKey(p => p.PaymentMethodId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.ProcessedByUser)
            .WithMany(u => u.Payments)
            .HasForeignKey(p => p.ProcessedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}