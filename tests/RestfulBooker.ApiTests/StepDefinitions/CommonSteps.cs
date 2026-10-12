using RestfulBooker.ApiTests.Support;

namespace RestfulBooker.ApiTests.StepDefinitions;

[Binding]
public sealed class CommonSteps(ScenarioData data)
{
    [Then("the response status code should be {int}")]
    public void ThenTheResponseStatusCodeShouldBe(int expected)
    {
        Assert.That((int)data.Response.StatusCode, Is.EqualTo(expected),
            $"Unexpected status code. Body: {data.Response.Content}");
    }

    [Then("the response should match the {string} schema")]
    public void ThenTheResponseShouldMatchTheSchema(string schemaName)
    {
        var errors = SchemaValidator.Validate(data.Response.Content!, schemaName);
        Assert.That(errors, Is.Empty, $"Response does not match '{schemaName}' schema");
    }
}
