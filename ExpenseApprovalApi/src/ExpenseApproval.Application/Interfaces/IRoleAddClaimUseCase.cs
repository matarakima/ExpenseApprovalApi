namespace ExpenseApproval.Application.Interfaces;

public interface IRoleAddClaimUseCase
{
    Task ExecuteAsync(Guid roleId, string claimValue);
}
