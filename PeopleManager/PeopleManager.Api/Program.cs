using System.Text.RegularExpressions;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Web;

namespace PeopleManager.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            string port = "5060";

            if (args.Length == 2)
            {
                if (args[0].Trim() == "-p")
                {
                    var portStr = args[1];
                    //判断端口是否正确 纯数字验证
                    if (Regex.IsMatch(portStr, @"^\d*$"))
                    {
                        port = portStr;
                    }
                }
            }

            return WebHost.CreateDefaultBuilder(args)
                //.UseConfiguration(config)
                .UseUrls($"http://*:{port}")
                .UseStartup<Startup>()
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.SetMinimumLevel(LogLevel.Debug);
                    logging.AddConsole();
                }).UseNLog();
        }
    }
}
