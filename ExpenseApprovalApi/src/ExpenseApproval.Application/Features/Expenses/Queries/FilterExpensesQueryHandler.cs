using ExpenseApproval.Application.DTOs;
using ExpenseApproval.Application.Interfaces;
using MediatR;

namespace ExpenseApproval.Application.Features.Expenses.Queries;

public class FilterExpensesQueryHandler : IRequestHandler<FilterExpensesQuery, IEnumerable<ExpenseRequestDto>>
{
    private readonly IExpenseRequestFilterUseCase _useCase;

    public FilterExpensesQueryHandler(IExpenseRequestFilterUseCase useCase)
    {
        _useCase = useCase;
    }

    public async Task<IEnumerable<ExpenseRequestDto>> Handle(FilterExpensesQuery request, CancellationToken cancellationToken)
    {
        var filter = new FilterExpenseRequestDto(request.Status, request.Category, request.FromDate, request.ToDate);
        return await _useCase.ExecuteAsync(filter);
    }
}
