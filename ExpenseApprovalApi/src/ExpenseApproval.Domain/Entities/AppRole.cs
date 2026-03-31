using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseApproval.Domain.Entities
{
    public class AppRole
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public ICollection<AppRoleClaim> Claims { get; set; } = new List<AppRoleClaim>();
    }
}
