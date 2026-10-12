using RestfulBooker.Api.Models;
using RestSharp;

namespace RestfulBooker.Api.Clients;

public sealed class AuthClient(ApiClient api)
{
    public Task<RestResponse<AuthResponse>> CreateTokenAsync(string username, string password)
    {
        var request = new RestRequest("auth", Method.Post).AddJsonBody(new AuthRequest(username, password));
        return api.ExecuteAsync<AuthResponse>(request);
    }
}
