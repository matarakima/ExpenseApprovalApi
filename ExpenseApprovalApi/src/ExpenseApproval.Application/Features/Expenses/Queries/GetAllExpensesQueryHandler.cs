using ExpenseApproval.Application.DTOs;
using ExpenseApproval.Application.Interfaces;
using MediatR;

namespace ExpenseApproval.Application.Features.Expenses.Queries;

public class GetAllExpensesQueryHandler : IRequestHandler<GetAllExpensesQuery, IEnumerable<ExpenseRequestDto>>
{
    private readonly IExpenseRequestGetAllUseCase _useCase;

    public GetAllExpensesQueryHandler(IExpenseRequestGetAllUseCase useCase)
    {
        _useCase = useCase;
    }

    public async Task<IEnumerable<ExpenseRequestDto>> Handle(GetAllExpensesQuery request, CancellationToken cancellationToken)
    {
        return await _useCase.ExecuteAsync();
    }
}
