using Azure.Monitor.OpenTelemetry.Exporter;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System.Diagnostics;

namespace Genocs.FaceComparison.WebApi;

internal static class OpenTelemetryInitializer
{
    public static void Initialize(WebApplicationBuilder builder)
    {
        builder.Services.AddOpenTelemetryTracing(x =>
        {
            x.SetResourceBuilder(ResourceBuilder.CreateDefault()
                    .AddService("FaceComparisonApi")
                    .AddTelemetrySdk()
                    .AddEnvironmentVariableDetector())
                .AddAspNetCoreInstrumentation()
                .AddAzureMonitorTraceExporter(o =>
                {
                    o.ConnectionString = builder.Configuration["ApplicationInsightsConnectionString"];
                })
                .AddJaegerExporter(o =>
                {
                    o.AgentHost = "localhost";
                    o.AgentPort = 6831;
                    o.MaxPayloadSizeInBytes = 4096;
                    o.ExportProcessorType = ExportProcessorType.Batch;
                    o.BatchExportProcessorOptions = new BatchExportProcessorOptions<Activity>
                    {
                        MaxQueueSize = 2048,
                        ScheduledDelayMilliseconds = 5000,
                        ExporterTimeoutMilliseconds = 30000,
                        MaxExportBatchSize = 512,
                    };
                });
        });
    }
}
