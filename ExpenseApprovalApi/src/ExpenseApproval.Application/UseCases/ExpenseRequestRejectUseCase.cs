using ExpenseApproval.Application.DTOs;
using ExpenseApproval.Application.Interfaces;
using ExpenseApproval.Domain.Enums;
using ExpenseApproval.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace ExpenseApproval.Application.UseCases;

public class ExpenseRequestRejectUseCase : IExpenseRequestRejectUseCase
{
    private readonly IExpenseRequestRepository _repository;
    private readonly ILogger<ExpenseRequestRejectUseCase> _logger;

    public ExpenseRequestRejectUseCase(
        IExpenseRequestRepository repository,
        ILogger<ExpenseRequestRejectUseCase> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<ExpenseRequestDto> ExecuteAsync(Guid id, Guid decisionById)
    {
        var entity = await _repository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Expense request {id} not found.");

        if (entity.Status != ExpenseStatus.Pending)
            throw new InvalidOperationException("Only pending requests can be rejected.");

        entity.Status = ExpenseStatus.Rejected;
        entity.DecisionDate = DateTime.UtcNow;
        entity.DecisionById = decisionById;

        await _repository.UpdateAsync(entity);
        _logger.LogInformation("Expense request {Id} rejected by {User}", id, decisionById);
        return ExpenseRequestMapper.MapToDto(entity);
    }
}
