using System.Diagnostics;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace ItPlanet.Tracing;

internal static class OpenTelemetryTracing
{
    public static ActivitySource TracingActivitySource = new("Test");
    
    public static TracerProvider? Initialize(string serviceName, string version)
    {
        TracingActivitySource = new(serviceName, version);
        return Sdk.CreateTracerProviderBuilder()
            .SetResourceBuilder(ResourceBuilder.CreateDefault()
                .AddService(serviceName: serviceName, serviceVersion: version))
            .AddSource(serviceName)
            .AddAspNetCoreInstrumentation()
            .AddJaegerExporter()
            .AddConsoleExporter()
            .Build();
    }
}