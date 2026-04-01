namespace ExpenseApproval.Application.DTOs;

public record LoginRequestDto(string Email, string Password);

public record LoginResponseDto(Guid UserId, string AccessToken, string TokenType, int ExpiresIn);
