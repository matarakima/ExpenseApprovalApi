using ExpenseApproval.Domain.Entities;
using ExpenseApproval.Domain.Enums;
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
    public class ExpenseRequestRepository : IExpenseRequestRepository
    {
        private readonly AppDbContext _context;

        public ExpenseRequestRepository(AppDbContext context) => _context = context;

        public async Task<ExpenseRequest?> GetByIdAsync(Guid id)
            => await _context.ExpenseRequests
                .Include(x => x.Category)
                .Include(x => x.RequestedBy)
                .Include(x => x.DecisionBy)
                .FirstOrDefaultAsync(x => x.Id == id);

        public async Task<IEnumerable<ExpenseRequest>> GetAllAsync()
            => await _context.ExpenseRequests
                .Include(x => x.Category)
                .Include(x => x.RequestedBy)
                .Include(x => x.DecisionBy)
                .OrderByDescending(x => x.CreatedAt).ToListAsync();

        public async Task<IEnumerable<ExpenseRequest>> FilterAsync(
            ExpenseStatus? status, string? category, DateTime? from, DateTime? to)
        {
            var query = _context.ExpenseRequests.AsQueryable();

            if (status.HasValue)
                query = query.Where(x => x.Status == status.Value);
            if (!string.IsNullOrWhiteSpace(category))
                query = query.Where(x => x.Category.Name == category);
            if (from.HasValue)
                query = query.Where(x => x.ExpenseDate >= from.Value);
            if (to.HasValue)
                query = query.Where(x => x.ExpenseDate <= to.Value);

            return await query.OrderByDescending(x => x.CreatedAt).ToListAsync();
        }

        public async Task<ExpenseRequest> AddAsync(ExpenseRequest entity)
        {
            _context.ExpenseRequests.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task UpdateAsync(ExpenseRequest entity)
        {
            _context.ExpenseRequests.Update(entity);
            await _context.SaveChangesAsync();
        }
    }
}
