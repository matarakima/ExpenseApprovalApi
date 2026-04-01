using ExpenseApproval.Application.DTOs;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseApproval.Application.Validators
{
    public class CreateExpenseRequestValidator : AbstractValidator<CreateExpenseRequestDto>
    {
        public CreateExpenseRequestValidator()
        {
            RuleFor(x => x.CategoryId)
                .NotEmpty().WithMessage("Category is required.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required.");

            RuleFor(x => x.Amount)
                .GreaterThan(0).WithMessage("Amount must be greater than zero.");

            RuleFor(x => x.ExpenseDate)
                .LessThanOrEqualTo(DateTime.UtcNow.Date.AddDays(1))
                .WithMessage("Expense date cannot be in the future.");

            RuleFor(x => x.RequestedById)
                .NotEmpty().WithMessage("Requested by user is required.");
        }
    }
}
