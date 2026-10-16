using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace RaceDay.API.Services;

public class SessionAuthSchemeOptions : AuthenticationSchemeOptions { }

public class SessionAuthHandler : AuthenticationHandler<SessionAuthSchemeOptions>
{
    public SessionAuthHandler(
        IOptionsMonitor<SessionAuthSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder)
        : base(options, logger, encoder) { }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var userId = Context.Session.GetInt32("UserId");
        var role = Context.Session.GetString("Role");

        if (userId == null || string.IsNullOrEmpty(role))
            return Task.FromResult(AuthenticateResult.Fail("No active session."));

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.Value.ToString()),
            new Claim(ClaimTypes.Role, role)
        };

        var identity = new ClaimsIdentity(claims, "SessionAuth");
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, "SessionAuth");

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
