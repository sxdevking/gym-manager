using GymManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymManager.Infrastructure.Persistence.Configurations;
/// <summary>
/// Configuracion EF Core para la entidad Branch
/// </summary>
public class BranchConfiguration : IEntityTypeConfiguration<Branch>
{
    public void Configure(EntityTypeBuilder<Branch> builder)
    {
        builder.ToTable("branches");

        builder.HasKey(b => b.BranchId);

        // Propiedades con nombres DIFERENTES en SQL
        builder.Property(b => b.Code)
            .HasColumnName("branch_code")
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(b => b.Name)
            .HasColumnName("branch_name")
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(b => b.Address)
            .HasMaxLength(255);

        builder.Property(b => b.Phone)
            .HasMaxLength(20);

        builder.Property(b => b.Email)
            .HasMaxLength(100);

        // TimeOnly se mapea a TIME en PostgreSQL
        // Nombres diferentes en SQL
        builder.Property(b => b.OpenTime)
            .HasColumnName("opening_time");

        builder.Property(b => b.CloseTime)
            .HasColumnName("closing_time");

        // Indices
        builder.HasIndex(b => b.Code).IsUnique();

        // Relacion 1:1 con BranchSettings
        builder.HasOne(b => b.Settings)
            .WithOne(bs => bs.Branch)
            .HasForeignKey<BranchSettings>(bs => bs.BranchId)
            .OnDelete(DeleteBehavior.Cascade);

        // Ignorar propiedades calculadas y de auditoria que no existen
        builder.Ignore(b => b.DeletedBy);
        builder.Ignore(b => b.IsDeleted);

        // Filtro soft delete
        builder.HasQueryFilter(b => b.DeletedAt == null);
    }
}
