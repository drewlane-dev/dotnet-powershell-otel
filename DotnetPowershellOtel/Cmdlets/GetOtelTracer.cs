using Azure.Monitor.OpenTelemetry.Exporter;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace DotnetPowershell.Cmdlets;

using System.Management.Automation;


[Cmdlet(VerbsCommon.Get, "OtelTracer")]
public class GetOtelTracer : Cmdlet
{
    [Parameter(Mandatory = true)]
    public string Name { get; set; }
    
    protected override void ProcessRecord()
    {
        using var tracerProvider = Sdk.CreateTracerProviderBuilder()
            .AddSource("*")
            .SetResourceBuilder(
                ResourceBuilder.CreateDefault()
                    .AddService(serviceName: Name, serviceVersion: "1.0.0"))
            .AddConsoleExporter()
            .AddAzureMonitorTraceExporter(options =>
                options.ConnectionString = "changeme")
            .AddJaegerExporter()
            .Build();
        var tracer = tracerProvider.GetTracer(Name);
        var span = tracer.StartSpan(Name);
        GlobalTracer.Context = span.Context;
        WriteObject($"00-{span.Context.TraceId.ToHexString()}-{span.Context.SpanId.ToHexString()}-00");
        span.Dispose();
    }
}