namespace ExpenseApproval.Application.DTOs;

public record LoginRequestDto(string Email, string Password);

public record LoginResponseDto(string AccessToken, string TokenType, int ExpiresIn);
