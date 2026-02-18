using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OECLWebsite.Data.Entities;

namespace OECLWebsite.Data.Configurations;

public class ApprovalWorkflowConfiguration : IEntityTypeConfiguration<ApprovalWorkflow>
{
    public void Configure(EntityTypeBuilder<ApprovalWorkflow> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.EntityType).IsRequired().HasMaxLength(100);
        builder.Property(e => e.RequestedBy).IsRequired().HasMaxLength(450);
        builder.Property(e => e.CurrentApprover).HasMaxLength(450);
        builder.Property(e => e.ResolvedBy).HasMaxLength(450);

        builder.HasIndex(e => e.CurrentStatus);
        builder.HasIndex(e => e.CurrentApprover);

        builder.HasOne(e => e.Requester)
            .WithMany()
            .HasForeignKey(e => e.RequestedBy)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Approver)
            .WithMany()
            .HasForeignKey(e => e.CurrentApprover)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
