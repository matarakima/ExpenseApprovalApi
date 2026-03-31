namespace ExpenseApproval.Application.Interfaces;

public interface IRoleRemoveClaimUseCase
{
    Task ExecuteAsync(Guid roleId, string claimValue);
}
