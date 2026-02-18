using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OECLWebsite.Data.Entities;

namespace OECLWebsite.Data.Configurations;

public class ApprovalHistoryConfiguration : IEntityTypeConfiguration<ApprovalHistory>
{
    public void Configure(EntityTypeBuilder<ApprovalHistory> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.ActionBy).HasMaxLength(450);
        builder.Property(e => e.OldStatus).HasMaxLength(50);
        builder.Property(e => e.NewStatus).HasMaxLength(50);

        builder.HasOne(e => e.Workflow)
            .WithMany(w => w.History)
            .HasForeignKey(e => e.WorkflowId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
