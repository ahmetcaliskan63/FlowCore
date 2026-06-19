using FlowCore.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlowCore.Infrastructure.Context.Configurations
{
    public class StatusHistoryConfiguration : IEntityTypeConfiguration<StatusHistory>
    {
        public void Configure(EntityTypeBuilder<StatusHistory> builder)
        {
            builder.ToTable("StatusHistories");

            builder.HasOne(s => s.ChangedByUser)
                .WithMany()
                .HasForeignKey(s => s.ChangedByUserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
