using ExpenseApproval.Application.DTOs;
using ExpenseApproval.Application.Interfaces;
using ExpenseApproval.Domain.Interfaces;

namespace ExpenseApproval.Application.UseCases;

public class CategoryGetAllUseCase : ICategoryGetAllUseCase
{
    private readonly ICategoryRepository _repository;

    public CategoryGetAllUseCase(ICategoryRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<CategoryDto>> ExecuteAsync()
    {
        var items = await _repository.GetAllAsync();
        return items.Select(c => new CategoryDto(c.Id, c.Name));
    }
}
