using System.Text.Json.Serialization;

namespace RestfulBooker.Api.Models;

public sealed record AuthRequest(
    [property: JsonPropertyName("username")] string Username,
    [property: JsonPropertyName("password")] string Password);

/// <summary>
/// The API answers 200 for both outcomes: a token on success,
/// or a "reason" ("Bad credentials") on failure.
/// </summary>
public sealed record AuthResponse
{
    [JsonPropertyName("token")] public string? Token { get; init; }
    [JsonPropertyName("reason")] public string? Reason { get; init; }
}
