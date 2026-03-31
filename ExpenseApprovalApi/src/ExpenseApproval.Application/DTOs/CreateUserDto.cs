using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseApproval.Application.DTOs
{
    public record CreateUserDto(string Auth0Id, string Email, string FullName, Guid RoleId);
}
