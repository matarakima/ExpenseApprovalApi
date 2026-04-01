using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseApproval.Application.DTOs
{
    public record CreateExpenseRequestDto(
        Guid CategoryId,
        string Description,
        decimal Amount,
        DateTime ExpenseDate,
        Guid RequestedById
    );
}
