using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace WorkerServiceDemo
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private HttpClient httpClient;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            httpClient = new HttpClient();
            return base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var response = await httpClient.GetAsync("https://www.google.com/");

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Target website is up. Status code: {StatusCode}", response.StatusCode);
                }
                else
                {
                    _logger.LogError("Target Website is down. Status code: {StatusCode}", response.StatusCode);
                }

                await Task.Delay(5 * 1 * 1000, stoppingToken);
            }
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            httpClient.Dispose();
            _logger.LogInformation("Service has been stopped.");
            return base.StopAsync(cancellationToken);
        }
    }
}
