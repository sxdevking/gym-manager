using GymManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymManager.Infrastructure.Persistence.Configurations;

// ============================================================
// PAYMENT METHODS (11_payment_methods.sql)
// ============================================================
public class PaymentMethodConfiguration : IEntityTypeConfiguration<PaymentMethod>
{
    public void Configure(EntityTypeBuilder<PaymentMethod> builder)
    {
        builder.ToTable("paymentmethods");

        builder.HasKey(pm => pm.PaymentMethodId);

        // C#: Name -> SQL: method_name
        builder.Property(pm => pm.Name)
            .HasColumnName("method_name")
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(pm => pm.Description)
            .HasMaxLength(255);

        builder.Property(pm => pm.RequiresReference)
            .HasColumnName("requires_reference");

        // Indices
        builder.HasIndex(pm => pm.Name).IsUnique();

        // Ignorar propiedades de auditoria
        builder.Ignore(pm => pm.DeletedBy);
        builder.Ignore(pm => pm.IsDeleted);
        builder.Ignore(pm => pm.DeletedAt);
        builder.Ignore(pm => pm.UpdatedAt);
        builder.Ignore(pm => pm.UpdatedBy);
        builder.Ignore(pm => pm.CreatedBy);
    }
}