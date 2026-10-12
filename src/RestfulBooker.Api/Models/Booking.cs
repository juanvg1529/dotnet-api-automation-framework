using System.Text.Json.Serialization;

namespace RestfulBooker.Api.Models;

public sealed record Booking
{
    [JsonPropertyName("firstname")] public string FirstName { get; init; } = string.Empty;
    [JsonPropertyName("lastname")] public string LastName { get; init; } = string.Empty;
    [JsonPropertyName("totalprice")] public int TotalPrice { get; init; }
    [JsonPropertyName("depositpaid")] public bool DepositPaid { get; init; }
    [JsonPropertyName("bookingdates")] public BookingDates BookingDates { get; init; } = new();
    // Optional field: omitted from the request instead of being sent as null.
    [JsonPropertyName("additionalneeds")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? AdditionalNeeds { get; init; }
}

public sealed record BookingDates
{
    [JsonPropertyName("checkin")] public DateOnly CheckIn { get; init; }
    [JsonPropertyName("checkout")] public DateOnly CheckOut { get; init; }
}

/// <summary>Response of POST /booking.</summary>
public sealed record CreatedBooking
{
    [JsonPropertyName("bookingid")] public int BookingId { get; init; }
    [JsonPropertyName("booking")] public Booking Booking { get; init; } = new();
}

/// <summary>Item of GET /booking.</summary>
public sealed record BookingId
{
    [JsonPropertyName("bookingid")] public int Id { get; init; }
}
