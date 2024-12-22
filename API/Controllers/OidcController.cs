using API.authhelper;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;


[Route("api/auth")]
[ApiController]
public class OidcController : ControllerBase
{
    private readonly OidcHelper _oidcHelper;
    private static readonly Dictionary<string, string> CodeVerifiers = new();

    public OidcController(OidcHelper oidcHelper)
    {
        _oidcHelper = oidcHelper;
    }

    [HttpGet("login")]
    public IActionResult Login()
    {
        var state = Guid.NewGuid().ToString();
        var codeVerifier = OidcHelper.GenerateCodeVerifier();
        var codeChallenge = OidcHelper.GenerateCodeChallenge(codeVerifier);
        
        CodeVerifiers[state] = codeVerifier;

        var authorizationUrl = _oidcHelper.GenerateAuthorizationRequestUrl(state, codeChallenge);
        return Redirect(authorizationUrl);
    }

    [HttpGet("callback")]
    public async Task<IActionResult> Callback([FromQuery] string code, [FromQuery] string state)
    {
        if (!CodeVerifiers.TryGetValue(state, out var codeVerifier))
        {
            return Unauthorized("Invalid state.");
        }

        var tokens = await _oidcHelper.ExchangeAuthorizationCodeForTokens(code, codeVerifier);
        var userInfo = await _oidcHelper.FetchUserInfo(tokens["access_token"]);
        
        CodeVerifiers.Remove(state);

        return Ok(new { Tokens = tokens, UserInfo = userInfo });
    }
    
  
}