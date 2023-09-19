using System.Diagnostics;
using Azure.Monitor.OpenTelemetry.Exporter;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace DotnetPowershell.Cmdlets;

using System.Management.Automation;


[Cmdlet(VerbsLifecycle.Start, "OtelSpan")]
public class StartOtelSpan : Cmdlet
{
    [Parameter(Mandatory = true)]
    public string ParentId { get; set; }
    
    [Parameter(Mandatory = true)]
    public string Name { get; set; }
    
    public static int counter = 0;
    
    protected override void ProcessRecord()
    {
        using var tracerProvider = Sdk.CreateTracerProviderBuilder()
            .AddSource("*")
            .SetResourceBuilder(
                ResourceBuilder.CreateDefault()
                    .AddService(serviceName: "PowershellModule", serviceVersion: "1.0.0"))
            .AddConsoleExporter()
            .AddAzureMonitorTraceExporter(options =>
                options.ConnectionString = "changeme")
            .AddJaegerExporter()
            .Build();
        var tracer = tracerProvider.GetTracer("PowershellModule");
        var activityContext = ActivityContext.Parse(ParentId, null);
        var spanContext = new SpanContext(activityContext.TraceId, activityContext.SpanId, activityContext.TraceFlags);
        var span = tracer.StartSpan(Name, parentContext: spanContext );
        var spanId = $"00-{span.Context.TraceId.ToHexString()}-{span.Context.SpanId.ToHexString()}-00";
        GlobalTracer.SpanContexts[spanId] = span;
        WriteObject(spanId);
    }
}