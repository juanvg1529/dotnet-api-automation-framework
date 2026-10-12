using System.Net;
using RestfulBooker.Api.Clients;
using RestfulBooker.Api.Models;
using RestfulBooker.ApiTests.Support;

namespace RestfulBooker.ApiTests.StepDefinitions;

[Binding]
public sealed class BookingSteps(BookingClient bookings, ScenarioData data)
{
    // ---------- Given ----------

    [Given("a valid booking")]
    public void GivenAValidBooking() => UseBooking(BookingBuilder.AValidBooking().Build());

    [Given("a valid booking with deposit paid {string} and additional needs {string}")]
    public void GivenAValidBookingWith(string depositPaid, string additionalNeeds) =>
        UseBooking(BookingBuilder.AValidBooking()
            .WithDepositPaid(bool.Parse(depositPaid))
            .WithAdditionalNeeds(additionalNeeds)
            .Build());

    [Given("a valid booking with checkout before checkin")]
    public void GivenAValidBookingWithCheckoutBeforeCheckin() =>
        UseBooking(BookingBuilder.AValidBooking().WithCheckoutBeforeCheckin().Build());

    [Given("a booking payload without the {string} field")]
    public void GivenABookingPayloadWithout(string field)
    {
        data.Payload = BookingBuilder.AValidBooking().BuildWithout(field);
        data.ExpectedBooking = null;
    }

    [Given("an existing booking")]
    public async Task GivenAnExistingBooking()
    {
        GivenAValidBooking();
        await WhenICreateTheBooking();
        Assert.That(data.Response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "Precondition failed: booking was not created");
    }

    // ---------- When ----------

    [When("I ping the API")]
    public async Task WhenIPingTheApi() => await data.SendAsync(() => bookings.PingAsync());

    [When("I create the booking")]
    public async Task WhenICreateTheBooking()
    {
        var response = await data.SendAsync(() => bookings.CreateBookingAsync(data.Payload!));

        if (response.IsSuccessful && response.Data is { BookingId: > 0 } created)
        {
            data.BookingId = created.BookingId;
            data.CreatedBookingIds.Add(created.BookingId);
        }
    }

    [When("I replace the booking with new valid data")]
    public async Task WhenIReplaceTheBookingWithNewValidData()
    {
        var replacement = BookingBuilder.AValidBooking().Build();
        await data.SendAsync(() => bookings.UpdateBookingAsync(data.RequiredBookingId, replacement, data.Token));
        data.ExpectedBooking = replacement;
    }

    [When("I change the first name to {string} and the total price to {int}")]
    public async Task WhenIChangeTheFirstNameAndTotalPrice(string firstName, int totalPrice)
    {
        var changes = new { firstname = firstName, totalprice = totalPrice };
        await data.SendAsync(() => bookings.PartialUpdateBookingAsync(data.RequiredBookingId, changes, data.Token));
        data.ExpectedBooking = data.ExpectedBooking! with { FirstName = firstName, TotalPrice = totalPrice };
    }

    [When("I delete the booking")]
    public async Task WhenIDeleteTheBooking() =>
        await data.SendAsync(() => bookings.DeleteBookingAsync(data.RequiredBookingId, data.Token));

    [When("I search bookings by the guest's first and last name")]
    public async Task WhenISearchBookingsByGuestName() =>
        await data.SendAsync(() => bookings.GetBookingIdsAsync(data.ExpectedBooking!.FirstName, data.ExpectedBooking.LastName));

    [When("I search bookings for a guest that does not exist")]
    public async Task WhenISearchBookingsForAGuestThatDoesNotExist() =>
        await data.SendAsync(() => bookings.GetBookingIdsAsync($"nobody-{Guid.NewGuid():N}", $"none-{Guid.NewGuid():N}"));

    // ---------- Then ----------

    [Then("the created booking should match the request")]
    public void ThenTheCreatedBookingShouldMatchTheRequest()
    {
        var created = data.ResponseAs<CreatedBooking>();
        Assert.Multiple(() =>
        {
            Assert.That(created.BookingId, Is.GreaterThan(0));
            Assert.That(created.Booking, Is.EqualTo(data.ExpectedBooking));
        });
    }

    [Then("the response should contain the expected booking")]
    public void ThenTheResponseShouldContainTheExpectedBooking() =>
        Assert.That(data.ResponseAs<Booking>(), Is.EqualTo(data.ExpectedBooking));

    [Then("the booking can be retrieved with the same data")]
    public async Task ThenTheBookingCanBeRetrievedWithTheSameData()
    {
        var response = await bookings.GetBookingAsync(data.RequiredBookingId);
        Assert.Multiple(() =>
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response.Data, Is.EqualTo(data.ExpectedBooking), "Stored booking differs from what was sent");
        });
    }

    [Then("the booking should no longer exist")]
    public async Task ThenTheBookingShouldNoLongerExist()
    {
        var response = await bookings.GetBookingAsync(data.RequiredBookingId);
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        data.CreatedBookingIds.Remove(data.RequiredBookingId); // nothing left to clean up
    }

    [Then("the booking should still exist")]
    public async Task ThenTheBookingShouldStillExist()
    {
        var response = await bookings.GetBookingAsync(data.RequiredBookingId);
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Then("the search results should include the booking")]
    public void ThenTheSearchResultsShouldIncludeTheBooking()
    {
        var ids = data.ResponseAs<List<BookingId>>().Select(b => b.Id);
        Assert.That(ids, Does.Contain(data.RequiredBookingId));
    }

    [Then("the search results should be empty")]
    public void ThenTheSearchResultsShouldBeEmpty() =>
        Assert.That(data.ResponseAs<List<BookingId>>(), Is.Empty);

    private void UseBooking(Booking booking)
    {
        data.Payload = booking;
        data.ExpectedBooking = booking;
    }
}
