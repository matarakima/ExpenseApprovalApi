using ExpenseApproval.Domain.Entities;
using ExpenseApproval.Domain.Interfaces;
using ExpenseApproval.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ExpenseApproval.Infrastructure.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly AppDbContext _context;

    public CategoryRepository(AppDbContext context) => _context = context;

    public async Task<IEnumerable<Category>> GetAllAsync()
        => await _context.Categories.OrderBy(c => c.Name).ToListAsync();
}
