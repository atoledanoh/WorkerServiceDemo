using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using System;

namespace WorkerServiceDemo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            //adding the use of serilog
            .UseSerilog()
                .ConfigureServices((hostContext, services) =>
                {
                    //configuring the logger, taken from the Serilog Documentation page
                    // https://github.com/serilog/serilog-settings-configuration
                    Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(hostContext.Configuration).CreateLogger();
                    services.AddHttpClient();
                    services.AddHostedService<Worker>();
                });
    }
}
