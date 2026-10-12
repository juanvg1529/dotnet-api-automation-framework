using RestfulBooker.Api.Configuration;
using RestSharp;

namespace RestfulBooker.Api.Clients;

/// <summary>
/// Thin wrapper around a single RestClient. RestClient is thread-safe and meant
/// to be reused, so one instance is shared by every endpoint client.
/// </summary>
public sealed class ApiClient : IDisposable
{
    private readonly RestClient _client;

    public ApiClient(ApiSettings settings)
    {
        var options = new RestClientOptions(settings.BaseUrl)
        {
            Timeout = TimeSpan.FromSeconds(settings.TimeoutSeconds),
            ThrowOnAnyError = false // tests assert on status codes, including errors
        };

        _client = new RestClient(options);
        // Without this header the API sometimes answers in XML / plain text.
        _client.AddDefaultHeader("Accept", "application/json");
    }

    public Task<RestResponse<T>> ExecuteAsync<T>(RestRequest request) => _client.ExecuteAsync<T>(request);

    public Task<RestResponse> ExecuteAsync(RestRequest request) => _client.ExecuteAsync(request);

    public void Dispose() => _client.Dispose();
}
