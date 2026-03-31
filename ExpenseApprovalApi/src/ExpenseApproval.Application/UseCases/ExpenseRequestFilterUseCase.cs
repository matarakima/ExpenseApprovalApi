using ExpenseApproval.Application.DTOs;
using ExpenseApproval.Application.Interfaces;
using ExpenseApproval.Domain.Enums;
using ExpenseApproval.Domain.Interfaces;

namespace ExpenseApproval.Application.UseCases;

public class ExpenseRequestFilterUseCase : IExpenseRequestFilterUseCase
{
    private readonly IExpenseRequestRepository _repository;

    public ExpenseRequestFilterUseCase(IExpenseRequestRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<ExpenseRequestDto>> ExecuteAsync(FilterExpenseRequestDto filter)
    {
        ExpenseStatus? status = null;
        if (!string.IsNullOrWhiteSpace(filter.Status) && Enum.TryParse<ExpenseStatus>(filter.Status, true, out var parsed))
            status = parsed;

        var items = await _repository.FilterAsync(status, filter.Category, filter.FromDate, filter.ToDate);
        return items.Select(ExpenseRequestMapper.MapToDto);
    }
}
