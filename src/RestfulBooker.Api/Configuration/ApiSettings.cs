using Microsoft.Extensions.Configuration;

namespace RestfulBooker.Api.Configuration;

/// <summary>
/// Strongly typed settings. Values come from appsettings.json, an optional
/// appsettings.{TEST_ENV}.json and environment variables prefixed with RB_
/// (e.g. RB_Api__BaseUrl), so secrets never need to live in the repo.
/// </summary>
public sealed class ApiSettings
{
    public string BaseUrl { get; init; } = "https://restful-booker.herokuapp.com";
    public int TimeoutSeconds { get; init; } = 30;
    public string Username { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;

    public static ApiSettings Load(string basePath)
    {
        var environment = Environment.GetEnvironmentVariable("TEST_ENV") ?? "default";

        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile($"appsettings.{environment}.json", optional: true)
            .AddEnvironmentVariables(prefix: "RB_")
            .Build();

        return configuration.GetSection("Api").Get<ApiSettings>()
               ?? throw new InvalidOperationException("Missing 'Api' section in configuration.");
    }
}
