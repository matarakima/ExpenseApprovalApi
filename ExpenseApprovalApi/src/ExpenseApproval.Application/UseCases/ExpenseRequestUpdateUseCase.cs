using ExpenseApproval.Application.DTOs;
using ExpenseApproval.Application.Interfaces;
using ExpenseApproval.Domain.Enums;
using ExpenseApproval.Domain.Interfaces;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace ExpenseApproval.Application.UseCases;

public class ExpenseRequestUpdateUseCase : IExpenseRequestUpdateUseCase
{
    private readonly IExpenseRequestRepository _repository;
    private readonly IValidator<UpdateExpenseRequestDto> _validator;
    private readonly ILogger<ExpenseRequestUpdateUseCase> _logger;

    public ExpenseRequestUpdateUseCase(
        IExpenseRequestRepository repository,
        IValidator<UpdateExpenseRequestDto> validator,
        ILogger<ExpenseRequestUpdateUseCase> logger)
    {
        _repository = repository;
        _validator = validator;
        _logger = logger;
    }

    public async Task<ExpenseRequestDto> ExecuteAsync(Guid id, UpdateExpenseRequestDto dto)
    {
        var validation = await _validator.ValidateAsync(dto);
        if (!validation.IsValid)
            throw new ValidationException(validation.Errors);

        var entity = await _repository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Expense request {id} not found.");

        if (entity.Status != ExpenseStatus.Pending)
            throw new InvalidOperationException("Only pending requests can be edited.");

        entity.CategoryId = dto.CategoryId;
        entity.Description = dto.Description;
        entity.Amount = dto.Amount;
        entity.ExpenseDate = dto.ExpenseDate;

        await _repository.UpdateAsync(entity);
        _logger.LogInformation("Expense request {Id} updated", id);
        return ExpenseRequestMapper.MapToDto(entity);
    }
}
