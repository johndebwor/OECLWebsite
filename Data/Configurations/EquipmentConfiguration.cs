using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OECLWebsite.Data.Entities;

namespace OECLWebsite.Data.Configurations;

public class EquipmentConfiguration : IEntityTypeConfiguration<Equipment>
{
    public void Configure(EntityTypeBuilder<Equipment> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Title).IsRequired().HasMaxLength(300);
        builder.Property(e => e.ShortDescription).HasMaxLength(500);
        builder.Property(e => e.CatalogReference).HasMaxLength(200);
        builder.Property(e => e.AvailabilityNote).HasMaxLength(500);
        builder.Property(e => e.IconUrl).HasMaxLength(500);
        builder.Property(e => e.ImageUrl).HasMaxLength(500);
        builder.Property(e => e.ApprovedBy).HasMaxLength(450);

        builder.HasIndex(e => e.CategoryId);
        builder.HasIndex(e => e.ApprovalStatus);

        builder.HasOne(e => e.Category)
            .WithMany(c => c.Equipment)
            .HasForeignKey(e => e.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
