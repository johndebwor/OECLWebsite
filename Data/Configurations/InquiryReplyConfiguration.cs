using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OECLWebsite.Data.Entities;

namespace OECLWebsite.Data.Configurations;

public class InquiryReplyConfiguration : IEntityTypeConfiguration<InquiryReply>
{
    public void Configure(EntityTypeBuilder<InquiryReply> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Subject).IsRequired().HasMaxLength(500);
        builder.Property(e => e.Body).IsRequired();
        builder.Property(e => e.SentByUserId).IsRequired().HasMaxLength(450);
        builder.Property(e => e.SentByName).IsRequired().HasMaxLength(200);

        builder.HasIndex(e => e.ServiceInquiryId);

        builder.HasOne(e => e.ServiceInquiry)
            .WithMany(i => i.Replies)
            .HasForeignKey(e => e.ServiceInquiryId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
