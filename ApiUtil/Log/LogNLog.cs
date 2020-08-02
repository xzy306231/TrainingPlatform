using System;
using System.Collections.Generic;
using NLog;

namespace ApiUtil.Log
{
    /// <summary>
    /// 
    /// </summary>
    public class LogNLog : ILog
    {
        private static ILogger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 
        /// </summary>
        public LogNLog()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public void Debug(string message)
        {
            Logger logger = LogManager.GetLogger("RmqTarget");
            var logEventInfo = new LogEventInfo(LogLevel.Debug, "RmqLogMessage", $"{message}, generated at {DateTime.UtcNow}.");
            logEventInfo.Properties["EmployeeID"] = "5677";
            logEventInfo.Properties["EmployeeName"] = "Akshay Patel";
            logEventInfo.Properties["fields"] = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>("Address", "Hyderabad")
            };
            logger.Log(logEventInfo);
            //LogManager.Shutdown();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Error(string message)
        {
            logger.Error(message);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Information(string message)
        {
            logger.Info(message);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Warning(string message)
        {
            logger.Warn(message);
        }
    }
}
