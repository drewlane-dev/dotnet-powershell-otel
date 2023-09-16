using System.Management.Automation;
using Azure.Monitor.OpenTelemetry.Exporter;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

Console.WriteLine("Hello, World!");
using var tracerProvider = Sdk.CreateTracerProviderBuilder()
    .AddSource("PowershellRunner")
    .SetResourceBuilder(
        ResourceBuilder.CreateDefault()
            .AddService(serviceName: "PowershellRunner", serviceVersion: "1.0.0"))
    .AddConsoleExporter()
    .AddAzureMonitorTraceExporter(options =>
        options.ConnectionString = "changeme")
    .AddJaegerExporter()
    .Build();
using var ps = PowerShell.Create();
var tracer = tracerProvider.GetTracer("PowershellRunner");
using var span = tracer.StartSpan("AppModule");



var startingExecutionPolicy = PowerShell.Create().AddCommand("Get-ExecutionPolicy").Invoke();
var exPolicy = startingExecutionPolicy.First().ToString();

using var moduleSpan = tracer.StartSpan("SqlPackage", SpanKind.Internal, parentSpan: span);

// can be piped to application insights
ps.Streams.Information.DataAdded += (PSSenderInfo, EventArgs) =>
{
    var debugOutput = ps.Streams.Information[EventArgs.Index].ToString();
    Console.WriteLine(debugOutput);
};

ps.Streams.Error.DataAdded += (PSSenderInfo, EventArgs) =>
{
    var debugOutput = ps.Streams.Error[EventArgs.Index].ToString();
    moduleSpan.AddEvent("error_log", attributes: new SpanAttributes(new KeyValuePair<string, object>[]
    {
        new("message", debugOutput)
    }));
    Console.WriteLine(debugOutput);
};

ps
    .AddScript("D:/DotnetPowershell/script.ps1 -name 'Drew' -location 'Florida'");

var res = ps.Invoke();

Console.WriteLine();

