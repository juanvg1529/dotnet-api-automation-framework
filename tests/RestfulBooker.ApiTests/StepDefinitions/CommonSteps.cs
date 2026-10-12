using RestfulBooker.ApiTests.Support;

namespace RestfulBooker.ApiTests.StepDefinitions;

[Binding]
public sealed class CommonSteps(ScenarioData data, QualitySettings quality)
{
    [Then("the response status code should be {int}")]
    public void ThenTheResponseStatusCodeShouldBe(int expected)
    {
        Assert.That((int)data.Response.StatusCode, Is.EqualTo(expected),
            $"Unexpected status code. Body: {data.Response.Content}");
    }

    [Then("the response time should be within the agreed limit")]
    public void ThenTheResponseTimeShouldBeWithinTheAgreedLimit() =>
        ThenTheResponseTimeShouldBeBelow(quality.MaxResponseTimeMs);

    [Then("the response time should be below {int} ms")]
    public void ThenTheResponseTimeShouldBeBelow(int maxMilliseconds)
    {
        var elapsed = data.LastResponseTime ?? throw new InvalidOperationException("No request has been timed yet.");
        Assert.That(elapsed.TotalMilliseconds, Is.LessThan(maxMilliseconds),
            $"{data.Response.Request.Method} {data.Response.ResponseUri} took {elapsed.TotalMilliseconds:F0} ms");
    }

    [Then("the response should match the {string} schema")]
    public void ThenTheResponseShouldMatchTheSchema(string schemaName)
    {
        var errors = SchemaValidator.Validate(data.Response.Content!, schemaName);
        Assert.That(errors, Is.Empty, $"Response does not match '{schemaName}' schema");
    }
}
