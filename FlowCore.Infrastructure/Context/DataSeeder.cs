using FlowCore.Core.Entities;
using FlowCore.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FlowCore.Infrastructure.Context
{
    public static class DataSeeder
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();

            // Ensure database is created and migrated
            await context.Database.MigrateAsync();

            // 1. Ensure Roles
            var rolesToSeed = new[]
            {
                new Role { Id = Guid.NewGuid(), Name = "Admin", Description = "Sistem Yöneticisi", CreatedAt = DateTime.UtcNow },
                new Role { Id = Guid.NewGuid(), Name = "Employee", Description = "Standart Çalışan", CreatedAt = DateTime.UtcNow },
                new Role { Id = Guid.NewGuid(), Name = "Manager", Description = "Departman Yöneticisi", CreatedAt = DateTime.UtcNow },
                new Role { Id = Guid.NewGuid(), Name = "HR", Description = "İnsan Kaynakları Uzmanı", CreatedAt = DateTime.UtcNow },
                new Role { Id = Guid.NewGuid(), Name = "CEO", Description = "Genel Müdür", CreatedAt = DateTime.UtcNow }
            };

            foreach (var role in rolesToSeed)
            {
                if (!await context.Roles.AnyAsync(r => r.Name == role.Name))
                {
                    context.Roles.Add(role);
                }
            }
            await context.SaveChangesAsync();

            var adminRole = await context.Roles.FirstOrDefaultAsync(r => r.Name == "Admin");

            // 2. Ensure Departments
            var itDept = await context.Departments.FirstOrDefaultAsync(d => d.DepartmentName.Contains("IT") || d.DepartmentName.Contains("Bilgi"));
            if (itDept == null)
            {
                itDept = new Department { Id = Guid.NewGuid(), DepartmentName = "Bilgi Teknolojileri (IT)", CreatedAt = DateTime.UtcNow };
                context.Departments.Add(itDept);
                context.Departments.Add(new Department { Id = Guid.NewGuid(), DepartmentName = "İnsan Kaynakları (IK)", CreatedAt = DateTime.UtcNow });
                await context.SaveChangesAsync();
            }

            // 3. Ensure Admin User
            if (!context.Users.Any(u => u.Email == "admin@flowcore.com"))
            {
                var adminUser = new User
                {
                    Id = Guid.NewGuid(),
                    FullName = "Sistem Yöneticisi",
                    Email = "admin@flowcore.com",
                    Password = passwordHasher.Hash("Admin123!"),
                    DepartmentId = itDept.Id,
                    RoleId = adminRole.Id,
                    IsActive = true,
                    TotalLeaveCredits = 30,
                    RemainingLeaveCredits = 30,
                    CreatedAt = DateTime.UtcNow
                };

                context.Users.Add(adminUser);
                await context.SaveChangesAsync();
            }
        }
    }
}
