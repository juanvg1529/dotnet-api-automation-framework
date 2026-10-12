using System.Text.Json;
using System.Text.Json.Nodes;
using Bogus;
using RestfulBooker.Api.Models;

namespace RestfulBooker.ApiTests.Support;

/// <summary>
/// Builds realistic, unique bookings with Bogus. Unique data per scenario is
/// what lets features run in parallel against a shared public API.
/// </summary>
public sealed class BookingBuilder
{
    private static readonly string?[] AdditionalNeeds = ["Breakfast", "Late checkout", "Airport transfer", "Extra bed", null];

    private Booking _booking;

    private BookingBuilder(Booking booking) => _booking = booking;

    public static BookingBuilder AValidBooking()
    {
        var faker = new Faker();
        var checkIn = DateOnly.FromDateTime(DateTime.Today).AddDays(faker.Random.Int(1, 90));

        return new BookingBuilder(new Booking
        {
            FirstName = faker.Name.FirstName(),
            LastName = $"{faker.Name.LastName()}-{faker.Random.AlphaNumeric(6)}",
            TotalPrice = faker.Random.Int(50, 3000),
            DepositPaid = faker.Random.Bool(),
            BookingDates = new BookingDates { CheckIn = checkIn, CheckOut = checkIn.AddDays(faker.Random.Int(1, 14)) },
            AdditionalNeeds = faker.PickRandom(AdditionalNeeds)
        });
    }

    public BookingBuilder WithDepositPaid(bool depositPaid)
    {
        _booking = _booking with { DepositPaid = depositPaid };
        return this;
    }

    public BookingBuilder WithAdditionalNeeds(string? additionalNeeds)
    {
        _booking = _booking with { AdditionalNeeds = additionalNeeds };
        return this;
    }

    public BookingBuilder WithCheckoutBeforeCheckin()
    {
        var dates = _booking.BookingDates;
        _booking = _booking with { BookingDates = new BookingDates { CheckIn = dates.CheckOut, CheckOut = dates.CheckIn } };
        return this;
    }

    public Booking Build() => _booking;

    /// <summary>Raw JSON payload without the given field, for negative tests.</summary>
    public JsonObject BuildWithout(string field)
    {
        var json = JsonSerializer.SerializeToNode(_booking)!.AsObject();
        json.Remove(field);
        return json;
    }
}
