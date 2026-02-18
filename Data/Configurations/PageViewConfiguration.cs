using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OECLWebsite.Data.Entities;

namespace OECLWebsite.Data.Configurations;

public class PageViewConfiguration : IEntityTypeConfiguration<PageView>
{
    public void Configure(EntityTypeBuilder<PageView> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.PageUrl).HasMaxLength(500);
        builder.Property(e => e.PageTitle).HasMaxLength(300);
        builder.Property(e => e.UserId).HasMaxLength(450);
        builder.Property(e => e.SessionId).HasMaxLength(100);
        builder.Property(e => e.IPAddress).HasMaxLength(50);
        builder.Property(e => e.UserAgent).HasMaxLength(500);
        builder.Property(e => e.ReferrerUrl).HasMaxLength(500);

        builder.HasIndex(e => e.ViewDate);
        builder.HasIndex(e => e.PageUrl);
    }
}
