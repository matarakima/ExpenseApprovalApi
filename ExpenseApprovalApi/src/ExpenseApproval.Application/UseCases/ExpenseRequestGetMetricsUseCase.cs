using ExpenseApproval.Application.DTOs;
using ExpenseApproval.Application.Interfaces;
using ExpenseApproval.Domain.Enums;
using ExpenseApproval.Domain.Interfaces;

namespace ExpenseApproval.Application.UseCases;

public class ExpenseRequestGetMetricsUseCase : IExpenseRequestGetMetricsUseCase
{
    private readonly IExpenseRequestRepository _repository;

    public ExpenseRequestGetMetricsUseCase(IExpenseRequestRepository repository)
    {
        _repository = repository;
    }

    public async Task<ExpenseMetricsDto> ExecuteAsync()
    {
        var all = await _repository.GetAllAsync();
        var list = all.ToList();

        return new ExpenseMetricsDto(
            TotalRequests: list.Count,
            ApprovedCount: list.Count(x => x.Status == ExpenseStatus.Approved),
            RejectedCount: list.Count(x => x.Status == ExpenseStatus.Rejected),
            PendingCount: list.Count(x => x.Status == ExpenseStatus.Pending),
            TotalApprovedAmount: list.Where(x => x.Status == ExpenseStatus.Approved).Sum(x => x.Amount)
        );
    }
}
