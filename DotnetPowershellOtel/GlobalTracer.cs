using OpenTelemetry.Trace;

namespace DotnetPowershell;

public class GlobalTracer
{
    public static Dictionary<string, TelemetrySpan> SpanContexts { get; set; } = new Dictionary<string, TelemetrySpan>();
    public static SpanContext Context { get; set; }
}