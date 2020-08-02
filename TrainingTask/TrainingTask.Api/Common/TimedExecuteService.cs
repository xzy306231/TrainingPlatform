using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace TrainingTask.Api.Common
{
    /// <summary>
    /// 
    /// </summary>
    public class TimedExecuteService : BackgroundService
    {
        private readonly ILogger<TimedExecuteService> _logger;

        /// <summary>
        /// 
        /// </summary>
        public TimedExecuteService(ILogger<TimedExecuteService> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stoppingToken"></param>
        /// <returns></returns>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                _logger.LogInformation($"{DateTime.Now} BackgroundService: 启动");
                while (!stoppingToken.IsCancellationRequested)
                {
                    await Task.Delay(5000, stoppingToken);//启动后5秒执行一次(用于测试)
                    _logger.LogInformation($"{DateTime.Now} 执行逻辑");
                }
                _logger.LogInformation($"{DateTime.Now} BackgroundService: 停止");
            }
            catch (Exception e)
            {
                _logger.LogInformation(!stoppingToken.IsCancellationRequested
                    ? $"{DateTime.Now} BackgroundService:异常 {e.Message} {e.StackTrace}"
                    : $"{DateTime.Now} BackgroundService: 停止");
            }
        }
    }
}
