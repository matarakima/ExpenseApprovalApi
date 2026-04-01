using ExpenseApproval.Application.DTOs;
using MediatR;

namespace ExpenseApproval.Application.Features.Categories.Queries;

public record GetAllCategoriesQuery : IRequest<IEnumerable<CategoryDto>>;
