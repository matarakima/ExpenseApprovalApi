using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseApproval.Domain.Entities
{
    public class AppUser
    {
        public Guid Id { get; set; }
        public string Auth0Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public Guid RoleId { get; set; }
        public AppRole Role { get; set; } = null!;
    }
}
