namespace API.authhelper;

using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text.Json;
using Microsoft.IdentityModel.Tokens;

public class OidcHelper
{
    private readonly string _clientId;
    private readonly string _clientSecret;
    private readonly string _redirectUri;
    private readonly string _authorizationEndpoint;
    private readonly string _tokenEndpoint;
    private readonly string _userinfoEndpoint;

    public OidcHelper(IConfiguration configuration)
    {
        _clientId = configuration["OidcClientId"];
        _clientSecret = configuration["OidcClientSecret"];
        _redirectUri = configuration["OidcRedirectUri"];
        _authorizationEndpoint = configuration["OidcAuthorizationEndpoint"];
        _tokenEndpoint = configuration["OidcTokenEndpoint"];
        _userinfoEndpoint = configuration["OidcUserInfoEndpoint"];
    }

    public string GenerateAuthorizationRequestUrl(string state, string codeChallenge)
    {
        return $"{_authorizationEndpoint}?client_id={_clientId}&response_type=code&scope=openid profile email&redirect_uri={_redirectUri}&state={state}&code_challenge={codeChallenge}&code_challenge_method=S256";
    }

    public async Task<Dictionary<string, string>> ExchangeAuthorizationCodeForTokens(string code, string codeVerifier)
    {
        using var client = new HttpClient();
        var form = new Dictionary<string, string>
        {
            { "grant_type", "authorization_code" },
            { "code", code },
            { "redirect_uri", _redirectUri },
            { "client_id", _clientId },
            { "client_secret", _clientSecret },
            { "code_verifier", codeVerifier }
        };
        var response = await client.PostAsync(_tokenEndpoint, new FormUrlEncodedContent(form));
        response.EnsureSuccessStatusCode();
        var jsonResponse = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<Dictionary<string, string>>(jsonResponse)!;
    }

    public async Task<Dictionary<string, object>> FetchUserInfo(string accessToken)
    {
        using var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        var response = await client.GetAsync(_userinfoEndpoint);
        response.EnsureSuccessStatusCode();
        var jsonResponse = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<Dictionary<string, object>>(jsonResponse)!;
    }

    public static string GenerateCodeVerifier()
    {
        var random = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(random);
        return Base64UrlEncoder.Encode(random);
    }

    public static string GenerateCodeChallenge(string codeVerifier)
    {
        using var sha256 = SHA256.Create();
        var hash = sha256.ComputeHash(System.Text.Encoding.ASCII.GetBytes(codeVerifier));
        return Base64UrlEncoder.Encode(hash);
    }
}