using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace MonitoringService;

public static class TracingService
{
    public static OpenTelemetryBuilder Setup(this OpenTelemetryBuilder builder, string serviceName, string serviceVersion)
    {
        return builder.WithTracing(tcb =>
        {
            tcb
                .AddSource(serviceName)
                .AddZipkinExporter(c =>
                {
                    c.Endpoint = new Uri("http://zipkin:9411/api/v2/spans");
                })
                .AddConsoleExporter()
                .SetResourceBuilder(
                    ResourceBuilder.CreateDefault()
                        .AddService(serviceName: serviceName, serviceVersion: serviceVersion))
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddConsoleExporter();
        });
    }  
}