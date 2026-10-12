using System.Collections.Concurrent;
using System.Text.Json;
using Json.Schema;

namespace RestfulBooker.ApiTests.Support;

/// <summary>Validates JSON bodies against the schemas in the Schemas folder.</summary>
public static class SchemaValidator
{
    private static readonly string SchemasDirectory = Path.Combine(AppContext.BaseDirectory, "Schemas");
    private static readonly ConcurrentDictionary<string, JsonSchema> Cache = new();
    private static readonly Lazy<bool> SchemasRegistered = new(RegisterAll);

    public static IReadOnlyList<string> Validate(string json, string schemaName)
    {
        _ = SchemasRegistered.Value;
        var schema = Cache[schemaName];

        using var document = JsonDocument.Parse(json);
        var result = schema.Evaluate(document.RootElement, new EvaluationOptions { OutputFormat = OutputFormat.List });

        return result.IsValid
            ? []
            : (result.Details ?? [])
                .Where(detail => detail.Errors is { Count: > 0 })
                .SelectMany(detail => detail.Errors!.Select(error => $"{detail.InstanceLocation}: {error.Value}"))
                .ToList();
    }

    // Schemas reference each other by $id (e.g. "$ref": "booking.json"),
    // so all of them are loaded and registered up front.
    private static bool RegisterAll()
    {
        foreach (var file in Directory.GetFiles(SchemasDirectory, "*.json"))
        {
            var schema = JsonSchema.FromFile(file);
            SchemaRegistry.Global.Register(schema);
            Cache[Path.GetFileNameWithoutExtension(file)] = schema;
        }

        return true;
    }
}
