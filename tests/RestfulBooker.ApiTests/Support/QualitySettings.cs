namespace RestfulBooker.ApiTests.Support;

/// <summary>
/// Non-functional limits (ISO/IEC 25010 performance efficiency) read from the
/// "Quality" section, overridable per environment like any other setting.
/// </summary>
public sealed class QualitySettings
{
    public int MaxResponseTimeMs { get; init; } = 3000;
}
