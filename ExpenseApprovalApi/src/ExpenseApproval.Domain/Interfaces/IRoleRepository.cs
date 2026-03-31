using ExpenseApproval.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseApproval.Domain.Interfaces
{
    public interface IRoleRepository
    {
        Task<AppRole?> GetByIdAsync(Guid id);
        Task<AppRole?> GetByNameAsync(string name);
        Task<IEnumerable<AppRole>> GetAllAsync();
        Task<AppRole> AddAsync(AppRole role);
        Task UpdateAsync(AppRole role);
        Task AddClaimAsync(Guid roleId, string claimValue);
        Task RemoveClaimAsync(Guid roleId, string claimValue);
    }
}
