using ExpenseApproval.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseApproval.Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<AppUser?> GetByAuth0IdAsync(string auth0Id);
        Task<AppUser?> GetByEmailAsync(string email);
        Task<AppUser?> GetByIdAsync(Guid id);
        Task<IEnumerable<AppUser>> GetAllAsync();
        Task<AppUser> AddAsync(AppUser user);
        Task UpdateAsync(AppUser user);
    }
}
