using GymManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymManager.Infrastructure.Persistence.Configurations;

// ============================================================
// BRANCH SETTINGS (03_branch_settings.sql)
// ============================================================
public class BranchSettingsConfiguration : IEntityTypeConfiguration<BranchSettings>
{
    public void Configure(EntityTypeBuilder<BranchSettings> builder)
    {
        builder.ToTable("branchsettings");

        // La PK en SQL es setting_id, pero en C# usamos BranchId
        // Necesitamos mapear correctamente
        builder.HasKey(bs => bs.BranchId);

        // Propiedades
        builder.Property(bs => bs.BusinessName)
            .HasMaxLength(150);

        // C#: LogoBase64 -> SQL: logo_path
        builder.Property(bs => bs.LogoBase64)
            .HasColumnName("logo_path")
            .HasMaxLength(500);

        builder.Property(bs => bs.PrimaryColor)
            .HasMaxLength(7);

        builder.Property(bs => bs.SecondaryColor)
            .HasMaxLength(7);

        builder.Property(bs => bs.ReceiptFooter)
            .HasColumnType("text");

        builder.Property(bs => bs.TaxId)
            .HasMaxLength(50);

        // C#: TicketInfo -> SQL: receipt_header (mapeo aproximado)
        builder.Property(bs => bs.TicketInfo)
            .HasColumnName("receipt_header")
            .HasColumnType("text");

        // Ignorar propiedades de auditoria que no existen
        builder.Ignore(bs => bs.DeletedBy);
        builder.Ignore(bs => bs.IsDeleted);
        builder.Ignore(bs => bs.DeletedAt);
        builder.Ignore(bs => bs.CreatedBy);
        builder.Ignore(bs => bs.UpdatedBy);

        // Filtro soft delete
        builder.HasQueryFilter(bs => bs.DeletedAt == null);
    }
}