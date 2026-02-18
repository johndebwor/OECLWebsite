using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OECLWebsite.Data.Entities;

namespace OECLWebsite.Data.Configurations;

public class ContentPageConfiguration : IEntityTypeConfiguration<ContentPage>
{
    public void Configure(EntityTypeBuilder<ContentPage> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.PageKey).IsRequired().HasMaxLength(100);
        builder.Property(e => e.PageTitle).HasMaxLength(300);
        builder.Property(e => e.MetaDescription).HasMaxLength(500);
        builder.Property(e => e.MetaKeywords).HasMaxLength(500);
        builder.Property(e => e.ApprovedBy).HasMaxLength(450);

        builder.HasIndex(e => e.PageKey).IsUnique();
    }
}
