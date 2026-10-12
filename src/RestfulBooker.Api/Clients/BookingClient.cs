using RestfulBooker.Api.Models;
using RestSharp;

namespace RestfulBooker.Api.Clients;

/// <summary>
/// One method per endpoint. Methods return the raw RestResponse so tests can
/// assert on status code, headers and body; no assertions live here.
/// </summary>
public sealed class BookingClient(ApiClient api)
{
    public Task<RestResponse> PingAsync() =>
        api.ExecuteAsync(new RestRequest("ping"));

    public Task<RestResponse<List<BookingId>>> GetBookingIdsAsync(string? firstName = null, string? lastName = null)
    {
        var request = new RestRequest("booking");
        if (firstName is not null) request.AddQueryParameter("firstname", firstName);
        if (lastName is not null) request.AddQueryParameter("lastname", lastName);
        return api.ExecuteAsync<List<BookingId>>(request);
    }

    public Task<RestResponse<Booking>> GetBookingAsync(int id) =>
        api.ExecuteAsync<Booking>(new RestRequest($"booking/{id}"));

    public Task<RestResponse<CreatedBooking>> CreateBookingAsync(object booking) =>
        api.ExecuteAsync<CreatedBooking>(new RestRequest("booking", Method.Post).AddJsonBody(booking));

    public Task<RestResponse<Booking>> UpdateBookingAsync(int id, Booking booking, string? token) =>
        api.ExecuteAsync<Booking>(WithToken(new RestRequest($"booking/{id}", Method.Put).AddJsonBody(booking), token));

    public Task<RestResponse<Booking>> PartialUpdateBookingAsync(int id, object changes, string? token) =>
        api.ExecuteAsync<Booking>(WithToken(new RestRequest($"booking/{id}", Method.Patch).AddJsonBody(changes), token));

    public Task<RestResponse> DeleteBookingAsync(int id, string? token) =>
        api.ExecuteAsync(WithToken(new RestRequest($"booking/{id}", Method.Delete), token));

    // Restful-Booker authenticates write operations with a "token" cookie.
    private static RestRequest WithToken(RestRequest request, string? token) =>
        token is null ? request : request.AddHeader("Cookie", $"token={token}");
}
