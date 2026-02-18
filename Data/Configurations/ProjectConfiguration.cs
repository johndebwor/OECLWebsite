using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OECLWebsite.Data.Entities;

namespace OECLWebsite.Data.Configurations;

public class ProjectConfiguration : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.ProjectName).IsRequired().HasMaxLength(300);
        builder.Property(e => e.Client).HasMaxLength(200);
        builder.Property(e => e.Location).HasMaxLength(300);
        builder.Property(e => e.ProjectType).HasMaxLength(100);
        builder.Property(e => e.ProjectValue).HasPrecision(18, 2);
        builder.Property(e => e.FeaturedImageUrl).HasMaxLength(500);
        builder.Property(e => e.ApprovedBy).HasMaxLength(450);

        builder.HasIndex(e => e.Status);
        builder.HasIndex(e => e.ApprovalStatus);
    }
}
