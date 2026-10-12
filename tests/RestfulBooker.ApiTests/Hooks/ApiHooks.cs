using System.Text;
using Allure.Net.Commons;
using RestfulBooker.Api.Clients;
using RestfulBooker.Api.Configuration;
using RestfulBooker.ApiTests.Support;
using Reqnroll.BoDi;

namespace RestfulBooker.ApiTests.Hooks;

[Binding]
public sealed class ApiHooks(IObjectContainer container, ScenarioData data)
{
    private static ApiSettings _settings = null!;
    private static ApiClient _api = null!;

    [BeforeTestRun]
    public static void CreateSharedClient()
    {
        _settings = ApiSettings.Load(AppContext.BaseDirectory);
        _api = new ApiClient(_settings);
    }

    [AfterTestRun]
    public static void DisposeSharedClient() => _api.Dispose();

    /// <summary>
    /// Registers the shared instances so step classes can receive
    /// AuthClient / BookingClient through their constructors.
    /// </summary>
    [BeforeScenario(Order = 0)]
    public void RegisterDependencies()
    {
        container.RegisterInstanceAs(_settings);
        container.RegisterInstanceAs(_api, dispose: false);
    }

    [AfterScenario(Order = 0)]
    public void AttachLastResponse()
    {
        if (data.LastResponse is not { } response) return;

        var summary = $"{response.Request.Method} {response.ResponseUri}\n" +
                      $"Status: {(int)response.StatusCode} {response.StatusCode}\n\n{response.Content}";
        AllureApi.AddAttachment("Last response", "text/plain", Encoding.UTF8.GetBytes(summary), ".txt");
    }

    /// <summary>Deletes every booking the scenario created, keeping the shared API clean.</summary>
    [AfterScenario(Order = 100)]
    public async Task DeleteCreatedBookings()
    {
        if (data.CreatedBookingIds.Count == 0) return;

        var bookings = container.Resolve<BookingClient>();
        var token = await AuthTokenCache.GetAsync(container.Resolve<AuthClient>(), _settings);

        foreach (var id in data.CreatedBookingIds)
        {
            await bookings.DeleteBookingAsync(id, token);
        }
    }
}
