using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace WorkerServiceDemo
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> logger;
        private readonly IHttpClientFactory httpClientFactory;
        private List<string> Urls = new List<string> {
            "https://google.com",
            "https://youtube.com",
            "https://facebook.com"
        };

        /// <summary>
        /// Using Microsoft.Extensions.Http's  Http Client Factory
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="httpClientFactory"></param>
        public Worker(ILogger<Worker> logger, IHttpClientFactory httpClientFactory)
        {
            this.logger = logger;
            this.httpClientFactory = httpClientFactory;
        }

        /// <summary>
        /// Method override used only to log the start of the process
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override Task StartAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Service has been started.");
            return base.StartAsync(cancellationToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stoppingToken"></param>
        /// <returns></returns>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await PollUrls();
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "An error has ocurred while polling the URLs");
                }
                finally
                {
                    await Task.Delay(1 * 1 * 1000, stoppingToken);
                }
            }
        }

        /// <summary>
        /// Method override used only to log the stopping of the process
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override Task StopAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Service has been stopped.");
            return base.StopAsync(cancellationToken);
        }

        /// <summary>
        /// Creates a list of tasks and populates them with one PollUrl task per URL in our List<string> Urls
        /// </summary>
        /// <returns></returns>
        private async Task PollUrls()
        {
            var tasks = new List<Task>();
            foreach (var url in Urls)
            {
                tasks.Add(PollUrl(url));
            }
            await Task.WhenAll(tasks);
        }

        /// <summary>
        /// Task that logs whether the response status is successful or not
        /// </summary>
        /// <returns></returns>
        private async Task PollUrl(string url)
        {
            try
            {
                var client = httpClientFactory.CreateClient();
                var response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    logger.LogInformation("{Url} is online.", url);
                }
                else
                {
                    logger.LogWarning("{Url} is offline.", url);
                }
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "{Url} is offline.", url);
            }
        }
    }
}
