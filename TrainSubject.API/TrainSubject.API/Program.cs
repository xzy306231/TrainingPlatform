using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace TrainSubject.API
{
    public class Program
    {
        private static readonly IConfigurationBuilder ConfigurationBuilder = new ConfigurationBuilder();
        private static IConfigurationRoot _configuration;
        public static void Main(string[] args)
        {
            _configuration = ConfigurationBuilder.SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();
            string port = _configuration.GetSection("eureka:instance:port").Value;
            CreateWebHostBuilder(args, port).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args, string port) =>
          WebHost.CreateDefaultBuilder(args).UseUrls($"http://*:{port}")
              .UseStartup<Startup>();
    }
}
