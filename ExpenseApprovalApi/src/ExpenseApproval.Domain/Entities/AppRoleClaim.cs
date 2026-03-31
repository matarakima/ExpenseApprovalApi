using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseApproval.Domain.Entities
{
    public class AppRoleClaim
    {
        public Guid Id { get; set; }
        public Guid RoleId { get; set; }
        public string ClaimValue { get; set; } = string.Empty; // e.g. "expenses:create", "expenses:approve"
        public AppRole Role { get; set; } = null!;
    }
}
