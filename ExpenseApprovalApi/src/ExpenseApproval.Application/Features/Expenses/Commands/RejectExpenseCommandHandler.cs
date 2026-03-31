using ExpenseApproval.Application.DTOs;
using ExpenseApproval.Application.Interfaces;
using MediatR;

namespace ExpenseApproval.Application.Features.Expenses.Commands;

public class RejectExpenseCommandHandler : IRequestHandler<RejectExpenseCommand, ExpenseRequestDto>
{
    private readonly IExpenseRequestRejectUseCase _useCase;

    public RejectExpenseCommandHandler(IExpenseRequestRejectUseCase useCase)
    {
        _useCase = useCase;
    }

    public async Task<ExpenseRequestDto> Handle(RejectExpenseCommand request, CancellationToken cancellationToken)
    {
        return await _useCase.ExecuteAsync(request.Id, request.DecisionById);
    }
}
