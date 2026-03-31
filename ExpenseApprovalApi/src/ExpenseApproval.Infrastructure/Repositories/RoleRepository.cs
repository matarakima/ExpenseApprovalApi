using ExpenseApproval.Domain.Entities;
using ExpenseApproval.Domain.Interfaces;
using ExpenseApproval.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseApproval.Infrastructure.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly AppDbContext _context;

        public RoleRepository(AppDbContext context) => _context = context;

        public async Task<AppRole?> GetByIdAsync(Guid id)
            => await _context.AppRoles.Include(r => r.Claims).FirstOrDefaultAsync(r => r.Id == id);

        public async Task<AppRole?> GetByNameAsync(string name)
            => await _context.AppRoles.Include(r => r.Claims).FirstOrDefaultAsync(r => r.Name == name);

        public async Task<IEnumerable<AppRole>> GetAllAsync()
            => await _context.AppRoles.Include(r => r.Claims).ToListAsync();

        public async Task<AppRole> AddAsync(AppRole role)
        {
            _context.AppRoles.Add(role);
            await _context.SaveChangesAsync();
            return role;
        }

        public async Task UpdateAsync(AppRole role)
        {
            _context.AppRoles.Update(role);
            await _context.SaveChangesAsync();
        }

        public async Task AddClaimAsync(Guid roleId, string claimValue)
        {
            var role = await _context.AppRoles.Include(r => r.Claims).FirstOrDefaultAsync(r => r.Id == roleId)
                ?? throw new KeyNotFoundException($"Role {roleId} not found.");

            if (role.Claims.Any(c => c.ClaimValue == claimValue))
                return;

            _context.AppRoleClaims.Add(new AppRoleClaim
            {
                Id = Guid.NewGuid(),
                RoleId = roleId,
                ClaimValue = claimValue
            });
            await _context.SaveChangesAsync();
        }

        public async Task RemoveClaimAsync(Guid roleId, string claimValue)
        {
            var claim = await _context.AppRoleClaims
                .FirstOrDefaultAsync(c => c.RoleId == roleId && c.ClaimValue == claimValue);

            if (claim is not null)
            {
                _context.AppRoleClaims.Remove(claim);
                await _context.SaveChangesAsync();
            }
        }
    }
}
