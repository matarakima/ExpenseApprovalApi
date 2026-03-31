using ExpenseApproval.Application.DTOs;
using ExpenseApproval.Application.Interfaces;
using ExpenseApproval.Domain.Entities;
using ExpenseApproval.Domain.Interfaces;

namespace ExpenseApproval.Application.UseCases;

public class ExpenseRequestGetAllUseCase : IExpenseRequestGetAllUseCase
{
    private readonly IExpenseRequestRepository _repository;

    public ExpenseRequestGetAllUseCase(IExpenseRequestRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<ExpenseRequestDto>> ExecuteAsync()
    {
        var items = await _repository.GetAllAsync();
        return items.Select(ExpenseRequestMapper.MapToDto);
    }
}
