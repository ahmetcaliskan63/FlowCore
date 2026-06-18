using FlowCore.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlowCore.Infrastructure.Context.Configurations
{
    public class ApprovalConfiguration : IEntityTypeConfiguration<Approval>
    {
        public void Configure(EntityTypeBuilder<Approval> builder)
        {
            builder.ToTable("Approvals");

            builder.Property(a => a.Comment)
                .HasMaxLength(500);

            builder.HasOne(a => a.ApproverByUser)
                .WithMany()
                .HasForeignKey(a => a.ApproverByUserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
