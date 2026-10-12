using RestfulBooker.Api.Clients;
using RestfulBooker.Api.Configuration;
using RestfulBooker.Api.Models;
using RestfulBooker.ApiTests.Support;

namespace RestfulBooker.ApiTests.StepDefinitions;

[Binding]
public sealed class AuthSteps(AuthClient auth, ApiSettings settings, ScenarioData data)
{
    [Given("I am authenticated")]
    public async Task GivenIAmAuthenticated() =>
        data.Token = await AuthTokenCache.GetAsync(auth, settings);

    [Given("I use the token {string}")]
    public void GivenIUseTheToken(string token) => data.Token = token;

    [When("I request a token with valid credentials")]
    public async Task WhenIRequestATokenWithValidCredentials() =>
        await data.SendAsync(() => auth.CreateTokenAsync(settings.Username, settings.Password));

    [When("I request a token with username {string} and password {string}")]
    public async Task WhenIRequestATokenWith(string username, string password) =>
        await data.SendAsync(() => auth.CreateTokenAsync(username, password));

    [Then("the response should contain a token")]
    public void ThenTheResponseShouldContainAToken()
    {
        var body = data.ResponseAs<AuthResponse>();
        Assert.That(body.Token, Is.Not.Null.And.Not.Empty);
    }

    [Then("the authentication should fail with reason {string}")]
    public void ThenTheAuthenticationShouldFailWithReason(string reason)
    {
        var body = data.ResponseAs<AuthResponse>();
        Assert.Multiple(() =>
        {
            Assert.That(body.Token, Is.Null, "No token should be issued");
            Assert.That(body.Reason, Is.EqualTo(reason));
        });
    }
}
