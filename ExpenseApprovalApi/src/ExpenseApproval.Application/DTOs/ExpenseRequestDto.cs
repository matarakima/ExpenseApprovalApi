using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseApproval.Application.DTOs
{
    public record ExpenseRequestDto(
        Guid Id,
        Guid CategoryId,
        string Category,
        string Description,
        decimal Amount,
        DateTime ExpenseDate,
        string RequestedBy,
        Guid RequestById,
        string Status,
        DateTime CreatedAt,
        DateTime? DecisionDate,
        string? DecisionBy,
        Guid? DecisionById
    );
}
