using ExpenseApproval.Application.DTOs;
using ExpenseApproval.Application.Interfaces;
using MediatR;

namespace ExpenseApproval.Application.Features.Expenses.Queries;

public class GetExpenseByIdQueryHandler : IRequestHandler<GetExpenseByIdQuery, ExpenseRequestDto?>
{
    private readonly IExpenseRequestGetByIdUseCase _useCase;

    public GetExpenseByIdQueryHandler(IExpenseRequestGetByIdUseCase useCase)
    {
        _useCase = useCase;
    }

    public async Task<ExpenseRequestDto?> Handle(GetExpenseByIdQuery request, CancellationToken cancellationToken)
    {
        return await _useCase.ExecuteAsync(request.Id);
    }
}
