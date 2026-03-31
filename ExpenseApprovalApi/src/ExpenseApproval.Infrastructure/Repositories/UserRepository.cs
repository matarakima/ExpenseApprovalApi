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
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context) => _context = context;

        public async Task<AppUser?> GetByAuth0IdAsync(string auth0Id)
            => await _context.AppUsers.Include(u => u.Role).ThenInclude(r => r.Claims)
                .FirstOrDefaultAsync(u => u.Auth0Id == auth0Id);

        public async Task<AppUser?> GetByIdAsync(Guid id)
            => await _context.AppUsers.Include(u => u.Role).ThenInclude(r => r.Claims)
                .FirstOrDefaultAsync(u => u.Id == id);

        public async Task<IEnumerable<AppUser>> GetAllAsync()
            => await _context.AppUsers.Include(u => u.Role).ThenInclude(r => r.Claims).ToListAsync();

        public async Task<AppUser> AddAsync(AppUser user)
        {
            _context.AppUsers.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task UpdateAsync(AppUser user)
        {
            _context.AppUsers.Update(user);
            await _context.SaveChangesAsync();
        }
    }
}
