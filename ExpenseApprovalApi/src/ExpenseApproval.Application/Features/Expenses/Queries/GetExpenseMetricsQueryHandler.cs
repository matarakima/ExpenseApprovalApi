using ExpenseApproval.Application.DTOs;
using ExpenseApproval.Application.Interfaces;
using MediatR;

namespace ExpenseApproval.Application.Features.Expenses.Queries;

public class GetExpenseMetricsQueryHandler : IRequestHandler<GetExpenseMetricsQuery, ExpenseMetricsDto>
{
    private readonly IExpenseRequestGetMetricsUseCase _useCase;

    public GetExpenseMetricsQueryHandler(IExpenseRequestGetMetricsUseCase useCase)
    {
        _useCase = useCase;
    }

    public async Task<ExpenseMetricsDto> Handle(GetExpenseMetricsQuery request, CancellationToken cancellationToken)
    {
        return await _useCase.ExecuteAsync();
    }
}
