using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseApproval.Application.DTOs
{
    public record UpdateExpenseRequestDto(
        Guid CategoryId,
        string Description,
        decimal Amount,
        DateTime ExpenseDate
    );
}
