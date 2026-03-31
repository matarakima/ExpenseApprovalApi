using ExpenseApproval.Application.DTOs;
using ExpenseApproval.Application.Interfaces;
using ExpenseApproval.Domain.Entities;
using ExpenseApproval.Domain.Enums;
using ExpenseApproval.Domain.Interfaces;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace ExpenseApproval.Application.UseCases;

public class ExpenseRequestCreateUseCase : IExpenseRequestCreateUseCase
{
    private readonly IExpenseRequestRepository _repository;
    private readonly IValidator<CreateExpenseRequestDto> _validator;
    private readonly ILogger<ExpenseRequestCreateUseCase> _logger;

    public ExpenseRequestCreateUseCase(
        IExpenseRequestRepository repository,
        IValidator<CreateExpenseRequestDto> validator,
        ILogger<ExpenseRequestCreateUseCase> logger)
    {
        _repository = repository;
        _validator = validator;
        _logger = logger;
    }

    public async Task<ExpenseRequestDto> ExecuteAsync(CreateExpenseRequestDto dto, Guid requestedById)
    {
        var validation = await _validator.ValidateAsync(dto);
        if (!validation.IsValid)
            throw new ValidationException(validation.Errors);

        var entity = new ExpenseRequest
        {
            Id = Guid.NewGuid(),
            CategoryId = dto.CategoryId,
            Description = dto.Description,
            Amount = dto.Amount,
            ExpenseDate = dto.ExpenseDate,
            RequestedById = requestedById,
            Status = ExpenseStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        var created = await _repository.AddAsync(entity);
        _logger.LogInformation("Expense request {Id} created by {User}", created.Id, requestedById);
        return ExpenseRequestMapper.MapToDto(created);
    }
}
