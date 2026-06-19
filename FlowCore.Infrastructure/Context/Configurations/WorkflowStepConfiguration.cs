using FlowCore.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlowCore.Infrastructure.Context.Configurations
{
    public class WorkflowStepConfiguration : IEntityTypeConfiguration<WorkflowStep>
    {
        public void Configure(EntityTypeBuilder<WorkflowStep> builder)
        {
            builder.ToTable("WorkflowSteps");

            builder.HasOne(ws => ws.Workflow)
                .WithMany()
                .HasForeignKey(ws => ws.WorkflowId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(ws => ws.RequiredRole)
                .WithMany()
                .HasForeignKey(ws => ws.RequiredRoleId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
