using System.Diagnostics;
using System.Text.Json;
using RestfulBooker.Api.Models;
using RestSharp;

namespace RestfulBooker.ApiTests.Support;

/// <summary>
/// Per-scenario state shared between step classes through Reqnroll's
/// context injection. A new instance is created for every scenario.
/// </summary>
public sealed class ScenarioData
{
    /// <summary>The payload sent in the last create/update request.</summary>
    public object? Payload { get; set; }

    /// <summary>The booking the scenario expects the API to hold.</summary>
    public Booking? ExpectedBooking { get; set; }

    public int? BookingId { get; set; }
    public string? Token { get; set; }
    public RestResponse? LastResponse { get; set; }
    public TimeSpan? LastResponseTime { get; set; }

    /// <summary>Bookings created by the scenario, deleted in the AfterScenario hook.</summary>
    public List<int> CreatedBookingIds { get; } = [];

    public RestResponse Response =>
        LastResponse ?? throw new InvalidOperationException("No request has been sent in this scenario yet.");

    public int RequiredBookingId =>
        BookingId ?? throw new InvalidOperationException("No booking has been created in this scenario yet.");

    /// <summary>Sends a request, keeping its response and how long it took.</summary>
    public async Task<RestResponse> SendAsync(Func<Task<RestResponse>> request)
    {
        var stopwatch = Stopwatch.StartNew();
        var response = await request();
        LastResponseTime = stopwatch.Elapsed;
        LastResponse = response;
        return response;
    }

    public async Task<RestResponse<T>> SendAsync<T>(Func<Task<RestResponse<T>>> request)
    {
        var stopwatch = Stopwatch.StartNew();
        var response = await request();
        LastResponseTime = stopwatch.Elapsed;
        LastResponse = response;
        return response;
    }

    public T ResponseAs<T>() =>
        JsonSerializer.Deserialize<T>(Response.Content ?? string.Empty)
        ?? throw new InvalidOperationException($"Response body could not be read as {typeof(T).Name}: {Response.Content}");
}
