using RestfulBooker.Api.Clients;
using RestfulBooker.Api.Configuration;

namespace RestfulBooker.ApiTests.Support;

/// <summary>
/// Requests the auth token once per test run and shares it across parallel
/// scenarios, instead of calling POST /auth before every write operation.
/// </summary>
public static class AuthTokenCache
{
    private static readonly SemaphoreSlim Lock = new(1, 1);
    private static string? _token;

    public static async Task<string> GetAsync(AuthClient auth, ApiSettings settings)
    {
        if (_token is not null) return _token;

        await Lock.WaitAsync();
        try
        {
            if (_token is not null) return _token;

            var response = await auth.CreateTokenAsync(settings.Username, settings.Password);
            _token = response.Data?.Token
                     ?? throw new InvalidOperationException($"Could not get an auth token: {response.StatusCode} {response.Content}");
            return _token;
        }
        finally
        {
            Lock.Release();
        }
    }
}
