using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseApproval.Application.DTOs
{
    public record UserDto(Guid Id, string Auth0Id, string Email, string FullName, string RoleName, IEnumerable<string> Claims);
}
