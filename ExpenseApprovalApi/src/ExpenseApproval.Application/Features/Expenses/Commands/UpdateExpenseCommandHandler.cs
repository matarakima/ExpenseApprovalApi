using ExpenseApproval.Application.DTOs;
using ExpenseApproval.Application.Interfaces;
using MediatR;

namespace ExpenseApproval.Application.Features.Expenses.Commands;

public class UpdateExpenseCommandHandler : IRequestHandler<UpdateExpenseCommand, ExpenseRequestDto>
{
    private readonly IExpenseRequestUpdateUseCase _useCase;

    public UpdateExpenseCommandHandler(IExpenseRequestUpdateUseCase useCase)
    {
        _useCase = useCase;
    }

    public async Task<ExpenseRequestDto> Handle(UpdateExpenseCommand request, CancellationToken cancellationToken)
    {
        var dto = new UpdateExpenseRequestDto(request.CategoryId, request.Description, request.Amount, request.ExpenseDate);
        return await _useCase.ExecuteAsync(request.Id, dto);
    }
}
