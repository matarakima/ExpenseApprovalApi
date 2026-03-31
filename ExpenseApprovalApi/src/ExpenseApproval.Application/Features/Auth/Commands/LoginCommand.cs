using ExpenseApproval.Application.DTOs;
using MediatR;

namespace ExpenseApproval.Application.Features.Auth.Commands;

public record LoginCommand(string Email, string Password) : IRequest<LoginResponseDto>;
