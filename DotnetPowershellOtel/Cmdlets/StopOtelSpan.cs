using System.Diagnostics;
using Azure.Monitor.OpenTelemetry.Exporter;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace DotnetPowershell.Cmdlets;

using System.Management.Automation;


[Cmdlet(VerbsLifecycle.Stop, "OtelSpan")]
public class StopOtelSpan : Cmdlet
{
    [Parameter(Mandatory = true)]
    public string SpanId { get; set; }

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
        GlobalTracer.SpanContexts[SpanId].Dispose();
    }
}