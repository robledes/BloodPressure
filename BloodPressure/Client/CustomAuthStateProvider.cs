using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;

namespace BloodPressure.Client;

public class CustomAuthStateProvider : AuthenticationStateProvider
{
    private readonly ILocalStorageService _localStorageService;
    private readonly HttpClient _http;

    public CustomAuthStateProvider(ILocalStorageService localStorageService, HttpClient http)
    {
        _localStorageService = localStorageService;
        _http = http;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        string? authToken = await _localStorageService.GetItemAsStringAsync("authToken");
        ClaimsIdentity identity = new();
        _http.DefaultRequestHeaders.Authorization = null;

        if (!string.IsNullOrEmpty(authToken))
            try
            {
                identity = new ClaimsIdentity(ParseClaimsFromJwt(authToken), "jwt");
                _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken.Replace("\"", ""));
            }
            catch
            {
                await _localStorageService.RemoveItemAsync("authToken");
                identity = new ClaimsIdentity();
            }

        ClaimsPrincipal? userClaims = new(identity);
        AuthenticationState? userState = new(userClaims);
        NotifyAuthenticationStateChanged(Task.FromResult(userState));
        return userState;
    }

    private static IEnumerable<Claim> ParseClaimsFromJwt(string authToken)
    {
        string base64 = authToken.Split('.')[1];

        switch (base64.Length % 4)
        {
            case 2: base64 += "=="; break;
            case 3: base64 += "="; break;
        }

        byte[] jsonBytes = Convert.FromBase64String(base64);
        Dictionary<string, object>? keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);
        return keyValuePairs.Select(k => new Claim(k.Key, k.Value.ToString()));
    }
}
