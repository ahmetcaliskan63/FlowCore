using FlowCore.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace FlowCore.Infrastructure.Context
{
    public class AppDbContext: DbContext
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

            modelBuilder.Entity<User>(Entity =>
            {
                Entity.Property(u => u.FullName).IsRequired().HasMaxLength(50);
                Entity.Property(u => u.Email).IsRequired().HasMaxLength(100);
                Entity.HasIndex(u => u.Email).IsUnique();
            });
        }

    }

}
