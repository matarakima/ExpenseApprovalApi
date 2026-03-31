using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseApproval.Application.DTOs
{
    public record ExpenseMetricsDto(
        int TotalRequests,
        int ApprovedCount,
        int RejectedCount,
        int PendingCount,
        decimal TotalApprovedAmount
    );
}
