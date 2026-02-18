using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OECLWebsite.Data.Entities;

namespace OECLWebsite.Data.Configurations;

public class ServiceInquiryConfiguration : IEntityTypeConfiguration<ServiceInquiry>
{
    public void Configure(EntityTypeBuilder<ServiceInquiry> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.CompanyName).HasMaxLength(200);
        builder.Property(e => e.ContactName).IsRequired().HasMaxLength(200);
        builder.Property(e => e.Email).IsRequired().HasMaxLength(200);
        builder.Property(e => e.Phone).HasMaxLength(50);
        builder.Property(e => e.ServiceInterest).HasMaxLength(200);
        builder.Property(e => e.AssignedTo).HasMaxLength(450);

        builder.HasIndex(e => e.Status);

        builder.HasOne(e => e.Service)
            .WithMany()
            .HasForeignKey(e => e.ServiceId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
