using FlowCore.Core.Common;
using FlowCore.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace FlowCore.Infrastructure.Context
{
    public class AppDbContext : DbContext
    {

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<AppTask> AppTasks { get; set; }
        public DbSet<LeaveRequest> LeaveRequests { get; set; }
        public DbSet<Approval> Approvals { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<StatusHistory> StatusHistories { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<Workflow> Workflows { get; set; }
        public DbSet<WorkflowStep> WorkflowSteps { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            modelBuilder.Entity<User>().HasQueryFilter(x => !x.IsDeleted);
            modelBuilder.Entity<Role>().HasQueryFilter(x => !x.IsDeleted);
            modelBuilder.Entity<Department>().HasQueryFilter(x => !x.IsDeleted);
            modelBuilder.Entity<AppTask>().HasQueryFilter(x => !x.IsDeleted);
            modelBuilder.Entity<LeaveRequest>().HasQueryFilter(x => !x.IsDeleted);
            modelBuilder.Entity<Approval>().HasQueryFilter(x => !x.IsDeleted);
            modelBuilder.Entity<Notification>().HasQueryFilter(x => !x.IsDeleted);
            modelBuilder.Entity<StatusHistory>().HasQueryFilter(x => !x.IsDeleted);
            modelBuilder.Entity<AuditLog>().HasQueryFilter(x => !x.IsDeleted);
            modelBuilder.Entity<Workflow>().HasQueryFilter(x => !x.IsDeleted);
            modelBuilder.Entity<WorkflowStep>().HasQueryFilter(x => !x.IsDeleted);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entries = ChangeTracker.Entries<BaseEntity>();

            foreach (var entry in entries)
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedAt = DateTime.UtcNow;
                        break;
                    case EntityState.Modified:
                        entry.Entity.UpdatedAt = DateTime.UtcNow;
                        break;
                }
            }

            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}
