using ExpenseApproval.Domain.Entities;
using ExpenseApproval.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseApproval.Infrastructure.Data
{
    public static class DatabaseSeeder
    {
        public static async Task SeedAsync(AppDbContext context, ILogger logger)
        {
            await context.Database.MigrateAsync();

            logger.LogInformation("Database migrated successfully.");

            if (await context.AppRoles.AnyAsync())
            {
                logger.LogInformation("Database already seeded. Skipping.");
                return;
            }

            logger.LogInformation("Seeding database...");

            // All available claims
            var allClaims = new[]
            {
            "expenses:list", "expenses:create", "expenses:read",
            "expenses:edit", "expenses:approve", "expenses:reject",
            "expenses:filter", "expenses:metrics",
            "users:list", "users:create", "users:read",
            "roles:list", "roles:create", "roles:read",
            "roles:add-claim", "roles:remove-claim"
        };

            // SuperAdmin role
            var superAdminRole = new AppRole
            {
                Id = Guid.NewGuid(),
                Name = "SuperAdmin",
                Claims = allClaims.Select(c => new AppRoleClaim
                {
                    Id = Guid.NewGuid(),
                    ClaimValue = c
                }).ToList()
            };

            // Approver role
            var approverRole = new AppRole
            {
                Id = Guid.NewGuid(),
                Name = "Approver",
                Claims = new[] { "expenses:list", "expenses:read", "expenses:approve", "expenses:reject", "expenses:filter", "expenses:metrics" }
                    .Select(c => new AppRoleClaim { Id = Guid.NewGuid(), ClaimValue = c }).ToList()
            };

            // Requester role
            var requesterRole = new AppRole
            {
                Id = Guid.NewGuid(),
                Name = "Requester",
                Claims = new[] { "expenses:list", "expenses:create", "expenses:read", "expenses:edit", "expenses:filter" }
                    .Select(c => new AppRoleClaim { Id = Guid.NewGuid(), ClaimValue = c }).ToList()
            };

            context.AppRoles.AddRange(superAdminRole, approverRole, requesterRole);

            // SuperAdmin user (Auth0Id will be updated after Auth0 config)
            var superAdmin = new AppUser
            {
                Id = Guid.NewGuid(),
                Auth0Id = "auth0|69cbc5cbd72aaebd88ab5b4d",
                Email = "admin@expenseapproval.com",
                FullName = "Super Administrator",
                RoleId = superAdminRole.Id
            };

            context.AppUsers.Add(superAdmin);

            // Seed categories
            var travelCategory = new Category { Id = Guid.NewGuid(), Name = "Viaje" };
            var officeCategory = new Category { Id = Guid.NewGuid(), Name = "Material" };
            var feedingCategory = new Category { Id = Guid.NewGuid(), Name = "Alimentacion" };
            context.Categories.AddRange(travelCategory, officeCategory, feedingCategory);

            // Seed sample expense requests
            context.ExpenseRequests.AddRange(
                new ExpenseRequest
                {
                    Id = Guid.NewGuid(),
                    CategoryId = travelCategory.Id,
                    Description = "Tiquete de vuelo a Bogota",
                    Amount = 950000m,
                    ExpenseDate = DateTime.UtcNow.AddDays(-5),
                    RequestedById = superAdmin.Id,
                    Status = ExpenseStatus.Pending,
                    CreatedAt = DateTime.UtcNow.AddDays(-5)
                },
                new ExpenseRequest
                {
                    Id = Guid.NewGuid(),
                    CategoryId = officeCategory.Id,
                    Description = "Tinta para impresora",
                    Amount = 120000m,
                    ExpenseDate = DateTime.UtcNow.AddDays(-10),
                    RequestedById = superAdmin.Id,
                    Status = ExpenseStatus.Approved,
                    CreatedAt = DateTime.UtcNow.AddDays(-10),
                    DecisionDate = DateTime.UtcNow.AddDays(-8),
                    DecisionById = superAdmin.Id
                },
                new ExpenseRequest
                {
                    Id = Guid.NewGuid(),
                    CategoryId = feedingCategory.Id,
                    Description = "Almuerzo con clientes",
                    Amount = 450000m,
                    ExpenseDate = DateTime.UtcNow.AddDays(-10),
                    RequestedById = superAdmin.Id,
                    Status = ExpenseStatus.Approved,
                    CreatedAt = DateTime.UtcNow.AddDays(-10),
                    DecisionDate = DateTime.UtcNow.AddDays(-8),
                    DecisionById = superAdmin.Id
                }
            );

            await context.SaveChangesAsync();
            logger.LogInformation("Database seeded successfully.");
        }
    }
}
