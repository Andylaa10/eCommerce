using Serilog;
using Serilog.Enrichers.Span;

namespace MonitoringService;

public static class LoggingService
{
    public static ILogger Log => Serilog.Log.Logger;
    
    static LoggingService()
    {
        // Serilog
        Serilog.Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .WriteTo.Seq("http://seq:5341")
            .WriteTo.Console()
            .Enrich.WithSpan()
            .CreateLogger();
    }
}