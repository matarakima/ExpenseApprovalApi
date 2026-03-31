using ExpenseApproval.Application.DTOs;
using ExpenseApproval.Application.Interfaces;
using ExpenseApproval.Domain.Interfaces;

namespace ExpenseApproval.Application.UseCases;

public class ExpenseRequestGetByIdUseCase : IExpenseRequestGetByIdUseCase
{
    private readonly IExpenseRequestRepository _repository;

    public ExpenseRequestGetByIdUseCase(IExpenseRequestRepository repository)
    {
        _repository = repository;
    }

    public async Task<ExpenseRequestDto?> ExecuteAsync(Guid id)
    {
        var entity = await _repository.GetByIdAsync(id);
        return entity is null ? null : ExpenseRequestMapper.MapToDto(entity);
    }
}
