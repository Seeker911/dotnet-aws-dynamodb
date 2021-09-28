using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DotNet5.Service.Exp.Domain;
using DotNet5.Service.Exp.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace DotNet5.Service.Exp
{
    public class Program
    {
        private static IHost _Host;

        public static async Task Main(string[] args)
        {
            using var processModule = Process.GetCurrentProcess().MainModule;
            if (processModule != null)
            {
                var pathToExe = processModule.FileName;
                var pathToContentRoot = Path.GetDirectoryName(pathToExe);
                Directory.SetCurrentDirectory(pathToContentRoot);
            }

            _Host = CreateHostBuilder(args).Build();
            await _Host.RunAsync();
        }

        /// <summary>
        /// Not sure how this would ever get called ?
        /// </summary>
        /// <returns></returns>
        public async Task StopAsync()
        {
            var logger = _Host.Services.GetService<ILogger>();

            logger.Information($"Worker - Stop signaled inside Program.cs");

            using (_Host)
            {
                await _Host.StopAsync();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>

            Host.CreateDefaultBuilder(args)
                .UseSystemd()
                .ConfigureHostConfiguration(config =>
                {
                    config.AddCommandLine(args);
                    config.AddEnvironmentVariables(prefix: "DOTNETCORE_");
                })
                .ConfigureAppConfiguration((hostContext, config) =>
                {
                    config.AddJsonFile(path: $"{Directory.GetCurrentDirectory()}\\appsettings.json", optional: false, reloadOnChange: true);
                    config.AddJsonFile(path: $"{Directory.GetCurrentDirectory()}\\appsettings.{hostContext.HostingEnvironment.EnvironmentName}.json", optional: true, reloadOnChange: true);
                })
                .ConfigureLogging((hostContext, logging) =>
                {
                    Log.Logger = new LoggerConfiguration()
                         .ReadFrom.Configuration(hostContext.Configuration)
                         .CreateLogger();

                    logging.AddSerilog();
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<Worker>()
                    .AddSingleton<AwsDbConfig>()
                    .AddSingleton<IBookRepository, BookRepository>()
                    .AddSingleton<IBookStore, BookStore>();

                });
    }
}
