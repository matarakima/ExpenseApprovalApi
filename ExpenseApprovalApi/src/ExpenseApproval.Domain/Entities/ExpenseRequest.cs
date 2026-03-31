using ExpenseApproval.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseApproval.Domain.Entities
{
    public class ExpenseRequest
    {
        public Guid Id { get; set; }
        public Guid CategoryId { get; set; }
        public Category Category { get; set; } = null!;
        public string Description { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime ExpenseDate { get; set; }
        public Guid RequestedById { get; set; }
        public AppUser RequestedBy { get; set; } = null!;
        public ExpenseStatus Status { get; set; } = ExpenseStatus.Pending;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? DecisionDate { get; set; }
        public Guid? DecisionById { get; set; }
        public AppUser? DecisionBy { get; set; }
    }
}
