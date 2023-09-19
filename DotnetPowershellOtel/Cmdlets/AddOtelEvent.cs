using System.Diagnostics;
using Azure.Monitor.OpenTelemetry.Exporter;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace DotnetPowershell.Cmdlets;

using System.Management.Automation;


[Cmdlet(VerbsCommon.Add, "OtelEvent")]
public class AddOtelEvent : Cmdlet
{
    [Parameter(Mandatory = true)]
    public string SpanId { get; set; }
    
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
        GlobalTracer.SpanContexts[SpanId].AddEvent(Name);
    }
}