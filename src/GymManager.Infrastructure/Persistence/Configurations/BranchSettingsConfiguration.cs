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

        // La PK real en SQL es setting_id
        builder.HasKey(bs => bs.SettingId);

        // ═══════════════════════════════════════════════════════════
        // MAPEO DE PROPIEDADES A COLUMNAS
        // ═══════════════════════════════════════════════════════════

        builder.Property(bs => bs.SettingId)
            .HasColumnName("setting_id");

        builder.Property(bs => bs.BranchId)
            .HasColumnName("branch_id")
            .IsRequired();

        // ═══════════════════════════════════════════════════════════
        // COLORES DE MARCA
        // ═══════════════════════════════════════════════════════════

        builder.Property(bs => bs.PrimaryColor)
            .HasColumnName("primary_color")
            .HasMaxLength(7);

        builder.Property(bs => bs.SecondaryColor)
            .HasColumnName("secondary_color")
            .HasMaxLength(7);

        builder.Property(bs => bs.AccentColor)
            .HasColumnName("accent_color")
            .HasMaxLength(7);

        // ═══════════════════════════════════════════════════════════
        // LOGOTIPOS
        // ═══════════════════════════════════════════════════════════

        builder.Property(bs => bs.LogoPath)
            .HasColumnName("logo_path")
            .HasMaxLength(500);

        builder.Property(bs => bs.LogoSmallPath)
            .HasColumnName("logo_small_path")
            .HasMaxLength(500);

        builder.Property(bs => bs.FaviconPath)
            .HasColumnName("favicon_path")
            .HasMaxLength(500);

        // ═══════════════════════════════════════════════════════════
        // INFORMACION FISCAL
        // ═══════════════════════════════════════════════════════════

        builder.Property(bs => bs.BusinessName)
            .HasColumnName("business_name")
            .HasMaxLength(150);

        builder.Property(bs => bs.TaxId)
            .HasColumnName("tax_id")
            .HasMaxLength(50);

        // ═══════════════════════════════════════════════════════════
        // PERSONALIZACION DE RECIBOS
        // ═══════════════════════════════════════════════════════════

        builder.Property(bs => bs.ReceiptHeader)
            .HasColumnName("receipt_header")
            .HasColumnType("text");

        builder.Property(bs => bs.ReceiptFooter)
            .HasColumnName("receipt_footer")
            .HasColumnType("text");

        // ═══════════════════════════════════════════════════════════
        // CONFIGURACION REGIONAL
        // ═══════════════════════════════════════════════════════════

        builder.Property(bs => bs.Timezone)
            .HasColumnName("timezone")
            .HasMaxLength(50);

        builder.Property(bs => bs.CurrencyCode)
            .HasColumnName("currency_code")
            .HasMaxLength(3);

        builder.Property(bs => bs.CurrencySymbol)
            .HasColumnName("currency_symbol")
            .HasMaxLength(5);

        builder.Property(bs => bs.DateFormat)
            .HasColumnName("date_format")
            .HasMaxLength(20);

        builder.Property(bs => bs.TimeFormat)
            .HasColumnName("time_format")
            .HasMaxLength(20);

        // ═══════════════════════════════════════════════════════════
        // HORARIOS DE OPERACION
        // ═══════════════════════════════════════════════════════════

        builder.Property(bs => bs.OpeningTime)
            .HasColumnName("opening_time");

        builder.Property(bs => bs.ClosingTime)
            .HasColumnName("closing_time");

        // ═══════════════════════════════════════════════════════════
        // AUDITORIA
        // ═══════════════════════════════════════════════════════════

        builder.Property(bs => bs.CreatedAt)
            .HasColumnName("created_at");

        builder.Property(bs => bs.UpdatedAt)
            .HasColumnName("updated_at");

        // Ignorar propiedades de auditoria que no existen en esta tabla
        builder.Ignore(bs => bs.IsActive);
        builder.Ignore(bs => bs.DeletedBy);
        builder.Ignore(bs => bs.IsDeleted);
        builder.Ignore(bs => bs.DeletedAt);
        builder.Ignore(bs => bs.CreatedBy);
        builder.Ignore(bs => bs.UpdatedBy);

        // ═══════════════════════════════════════════════════════════
        // INDICES
        // ═══════════════════════════════════════════════════════════

        builder.HasIndex(bs => bs.BranchId)
            .IsUnique()
            .HasDatabaseName("uq_branchsettings_branch_id");

        // ═══════════════════════════════════════════════════════════
        // RELACIONES
        // ═══════════════════════════════════════════════════════════

        // Relacion 1:1 con Branch
        builder.HasOne(bs => bs.Branch)
            .WithOne(b => b.Settings)
            .HasForeignKey<BranchSettings>(bs => bs.BranchId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
