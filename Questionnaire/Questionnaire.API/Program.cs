using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;

namespace Questionnaire.API
{
    public class Program
    {
        private static readonly IConfigurationBuilder ConfigurationBuilder = new ConfigurationBuilder();
        private static IConfigurationRoot _configuration;
        public static void Main(string[] args)
        {
            string path = Directory.GetCurrentDirectory();
            // 配置 Serilog 
            Log.Logger = new LoggerConfiguration()
            // 最小的日志输出级别
            .MinimumLevel.Debug()
            // 日志调用类命名空间如果以 Microsoft 开头，覆盖日志输出最小级别为 Information
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .Enrich.FromLogContext()
            // 配置日志输出到控制台
            .WriteTo.Console()
            // 配置日志输出到文件，文件输出到当前项目的 logs 目录下
            // 日记的生成周期为每天
            //.WriteTo.File(Path.Combine("ErrorLog", @"log.txt"), rollingInterval: RollingInterval.Day)
            // 创建 logger
            .CreateLogger();

            _configuration = ConfigurationBuilder.SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();
            string port = _configuration.GetSection("eureka:instance:port").Value;
            CreateWebHostBuilder(args, port).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args, string port) =>
          WebHost.CreateDefaultBuilder(args).UseSerilog().UseUrls($"http://*:{port}")
              .UseStartup<Startup>();
    }
}
