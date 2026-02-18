using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OECLWebsite.Data.Entities;

namespace OECLWebsite.Data.Configurations;

public class ProjectImageConfiguration : IEntityTypeConfiguration<ProjectImage>
{
    public void Configure(EntityTypeBuilder<ProjectImage> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.ImageUrl).IsRequired().HasMaxLength(500);
        builder.Property(e => e.Caption).HasMaxLength(500);

        builder.HasOne(e => e.Project)
            .WithMany(p => p.Images)
            .HasForeignKey(e => e.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
