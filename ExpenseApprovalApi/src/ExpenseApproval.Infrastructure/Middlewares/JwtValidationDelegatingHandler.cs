using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;

namespace ExpenseApproval.Infrastructure.Middlewares;

public class JwtValidationDelegatingHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public JwtValidationDelegatingHandler(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext is null)
            return UnauthorizedResponse();

        var result = await httpContext.AuthenticateAsync(JwtBearerDefaults.AuthenticationScheme);

        if (!result.Succeeded)
            return UnauthorizedResponse();

        return await base.SendAsync(request, cancellationToken);
    }

    private static HttpResponseMessage UnauthorizedResponse()
    {
        var payload = new { message = "Unauthorized. Please login again." };
        var content = JsonSerializer.Serialize(payload);

        return new HttpResponseMessage(HttpStatusCode.Unauthorized)
        {
            Content = new StringContent(content, System.Text.Encoding.UTF8, "application/json")
        };
    }
}
