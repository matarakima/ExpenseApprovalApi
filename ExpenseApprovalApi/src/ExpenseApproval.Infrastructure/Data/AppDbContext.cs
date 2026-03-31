using ExpenseApproval.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseApproval.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<ExpenseRequest> ExpenseRequests => Set<ExpenseRequest>();
        public DbSet<AppUser> AppUsers => Set<AppUser>();
        public DbSet<AppRole> AppRoles => Set<AppRole>();
        public DbSet<AppRoleClaim> AppRoleClaims => Set<AppRoleClaim>();
        public DbSet<Category> Categories => Set<Category>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ExpenseRequest>(e =>
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.CategoryId).IsRequired();
                e.HasOne(x => x.Category).WithMany().HasForeignKey(x => x.CategoryId);
                e.Property(x => x.Description).IsRequired().HasMaxLength(500);
                e.Property(x => x.Amount).HasColumnType("decimal(18,2)");
                e.Property(x => x.RequestedById).IsRequired();
                e.HasOne(x => x.RequestedBy).WithMany().HasForeignKey(x => x.RequestedById).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(x => x.DecisionBy).WithMany().HasForeignKey(x => x.DecisionById).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<AppRole>(e =>
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.Name).IsRequired().HasMaxLength(100);
                e.HasIndex(x => x.Name).IsUnique();
            });

            modelBuilder.Entity<AppRoleClaim>(e =>
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.ClaimValue).IsRequired().HasMaxLength(200);
                e.HasOne(x => x.Role).WithMany(r => r.Claims).HasForeignKey(x => x.RoleId);
            });

            modelBuilder.Entity<AppUser>(e =>
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.Auth0Id).IsRequired().HasMaxLength(200);
                e.HasIndex(x => x.Auth0Id).IsUnique();
                e.Property(x => x.Email).IsRequired().HasMaxLength(200);
                e.HasOne(x => x.Role).WithMany().HasForeignKey(x => x.RoleId);
            });

            modelBuilder.Entity<Category>(e =>
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.Name).IsRequired().HasMaxLength(100);
            });
        }
    }
}
