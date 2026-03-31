using ExpenseApproval.Domain.Entities;
using ExpenseApproval.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseApproval.Domain.Interfaces
{
    public interface IExpenseRequestRepository
    {
        Task<ExpenseRequest?> GetByIdAsync(Guid id);
        Task<IEnumerable<ExpenseRequest>> GetAllAsync();
        Task<IEnumerable<ExpenseRequest>> FilterAsync(ExpenseStatus? status, string? category, DateTime? from, DateTime? to);
        Task<ExpenseRequest> AddAsync(ExpenseRequest entity);
        Task UpdateAsync(ExpenseRequest entity);
    }
}
