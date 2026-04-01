using ExpenseApproval.Application.DTOs;
using ExpenseApproval.Application.Interfaces;
using MediatR;

namespace ExpenseApproval.Application.Features.Expenses.Commands;

public class CreateExpenseCommandHandler : IRequestHandler<CreateExpenseCommand, ExpenseRequestDto>
{
    private readonly IExpenseRequestCreateUseCase _useCase;

    public CreateExpenseCommandHandler(IExpenseRequestCreateUseCase useCase)
    {
        _useCase = useCase;
    }

    public async Task<ExpenseRequestDto> Handle(CreateExpenseCommand request, CancellationToken cancellationToken)
    {
        var dto = new CreateExpenseRequestDto(request.CategoryId, request.Description, request.Amount, request.ExpenseDate, request.RequestedById);
        return await _useCase.ExecuteAsync(dto);
    }
}
