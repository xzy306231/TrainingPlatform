using System.Text.RegularExpressions;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Web;

namespace PracticeBus
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            string port = "5080";

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
                    .UseUrls($"http://*:{port}")
                    .UseStartup<Startup>()
                    .ConfigureLogging(logging =>
                    {
                        logging.ClearProviders();
                        logging.SetMinimumLevel(LogLevel.Information);
                        logging.AddConsole();
                    }).UseNLog()
                ;
        }
    }
}
