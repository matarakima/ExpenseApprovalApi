using ExpenseApproval.Domain.Entities;

namespace ExpenseApproval.Domain.Interfaces;

public interface ICategoryRepository
{
    Task<IEnumerable<Category>> GetAllAsync();
}
