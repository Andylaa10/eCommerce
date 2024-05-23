using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace APIGateway.Authentication;

public class CustomRemoteAuthenticationOptions : AuthenticationSchemeOptions
{
    public CustomRemoteAuthenticationOptions()
    {
    }
}

public class CustomRemoteAuthenticationHandler : AuthenticationHandler<CustomRemoteAuthenticationOptions>
{
    private readonly HttpClient _httpClient;
    private readonly AppSettings.AppSettings _appSettings;
    public CustomRemoteAuthenticationHandler(IOptionsMonitor<CustomRemoteAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder, IOptions<AppSettings.AppSettings> appSettings) : base(options, logger, encoder)
    {
        _httpClient = new HttpClient();
        _appSettings = appSettings.Value;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var requestMessage = new HttpRequestMessage(HttpMethod.Get, _appSettings.ValidateTokenUrl);
        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", Request.Headers.Authorization.ToString().Replace("Bearer ", ""));

        var response = await _httpClient.SendAsync(requestMessage);
        var authResult = await response.Content.ReadFromJsonAsync<bool>();
        
        if (!authResult)
        {
            return AuthenticateResult.Fail("Invalid token");
        }   
        
        var claims = new List<Claim>();
        var claimsIdentity = new ClaimsIdentity(claims, "dev");
        var claimPrincipal = new ClaimsPrincipal(claimsIdentity);
        var authenticationTicket = new AuthenticationTicket(claimPrincipal, "dev");
        return AuthenticateResult.Success(authenticationTicket);
    }
}