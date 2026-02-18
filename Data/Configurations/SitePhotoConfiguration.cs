using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OECLWebsite.Data.Entities;

namespace OECLWebsite.Data.Configurations;

public class SitePhotoConfiguration : IEntityTypeConfiguration<SitePhoto>
{
    public void Configure(EntityTypeBuilder<SitePhoto> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.PhotoKey).IsRequired().HasMaxLength(100);
        builder.Property(e => e.ImageUrl).HasMaxLength(500);
        builder.Property(e => e.Title).HasMaxLength(200);
        builder.Property(e => e.Description).HasMaxLength(500);
        builder.Property(e => e.AltText).HasMaxLength(300);
        builder.HasIndex(e => e.PhotoKey).IsUnique();
    }
}
