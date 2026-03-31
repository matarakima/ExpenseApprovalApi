using ExpenseApproval.Application.DTOs;
using ExpenseApproval.Application.Interfaces;
using MediatR;

namespace ExpenseApproval.Application.Features.Expenses.Commands;

public class ApproveExpenseCommandHandler : IRequestHandler<ApproveExpenseCommand, ExpenseRequestDto>
{
    private readonly IExpenseRequestApproveUseCase _useCase;

    public ApproveExpenseCommandHandler(IExpenseRequestApproveUseCase useCase)
    {
        _useCase = useCase;
    }

    public async Task<ExpenseRequestDto> Handle(ApproveExpenseCommand request, CancellationToken cancellationToken)
    {
        return await _useCase.ExecuteAsync(request.Id, request.DecisionById);
    }
}
