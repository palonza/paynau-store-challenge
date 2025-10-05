
namespace Paynau.Infrastructure.Logging;

using System.IO;
using System.Threading;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.Http;

public static class LoggingExtensions
{
    public static ILogger CreateLogger(IConfiguration configuration)
    {
        var loggerConfig = new LoggerConfiguration()
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
            .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .Enrich.WithProperty("Application", "Paynau")
            .WriteTo.Console(
                outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}");

        var logtailEnabled = configuration.GetValue<bool>("LOGTAIL:ENABLED");
        var logtailToken = configuration.GetValue<string>("LOGTAIL:SOURCE_TOKEN");
        var logtailEndpoint = configuration.GetValue<string>("LOGTAIL:ENDPOINT") ?? "https://in.logtail.com";

        if (logtailEnabled && !string.IsNullOrWhiteSpace(logtailToken))
        {
            loggerConfig.WriteTo.Http(
                requestUri: logtailEndpoint,
                queueLimitBytes: null,
                httpClient: new LogtailHttpClient(logtailToken));
        }

        return loggerConfig.CreateLogger();
    }

    private class LogtailHttpClient : IHttpClient
    {
        private readonly HttpClient _httpClient;

        public LogtailHttpClient(string token)
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
        }

        public Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent content)
        {
            return _httpClient.PostAsync(requestUri, content);
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }

        public void Configure(IConfiguration configuration)
        {
            throw new NotImplementedException();
        }

        public Task<HttpResponseMessage> PostAsync(string requestUri, Stream contentStream, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
