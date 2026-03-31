using System.Net.Http.Json;
using System.Text.Json.Serialization;
using ExpenseApproval.Application.DTOs;
using MediatR;
using Microsoft.Extensions.Configuration;

namespace ExpenseApproval.Application.Features.Auth.Commands;

public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponseDto>
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    public LoginCommandHandler(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    public async Task<LoginResponseDto> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var domain = _configuration["Auth0:Domain"]
            ?? throw new InvalidOperationException("Auth0:Domain is not configured.");
        var clientId = _configuration["Auth0:ClientId"]
            ?? throw new InvalidOperationException("Auth0:ClientId is not configured.");
        var clientSecret = _configuration["Auth0:ClientSecret"]
            ?? throw new InvalidOperationException("Auth0:ClientSecret is not configured.");
        var audience = _configuration["Auth0:Audience"]
            ?? throw new InvalidOperationException("Auth0:Audience is not configured.");

        var client = _httpClientFactory.CreateClient();

        var tokenRequest = new
        {
            grant_type = "password",
            username = request.Email,
            password = request.Password,
            audience,
            client_id = clientId,
            client_secret = clientSecret,
            scope = "openid profile email"
        };

        var response = await client.PostAsJsonAsync(
            $"https://{domain}/oauth/token", tokenRequest, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new UnauthorizedAccessException("Invalid credentials.");
        }

        var tokenResponse = await response.Content.ReadFromJsonAsync<Auth0TokenResponse>(cancellationToken)
            ?? throw new InvalidOperationException("Failed to deserialize Auth0 token response.");

        return new LoginResponseDto(tokenResponse.AccessToken, tokenResponse.TokenType, tokenResponse.ExpiresIn);
    }

    private sealed class Auth0TokenResponse
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; } = string.Empty;

        [JsonPropertyName("token_type")]
        public string TokenType { get; set; } = string.Empty;

        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }
    }
}
