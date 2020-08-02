namespace ApiUtil.Log
{
    /// <summary>
    /// 
    /// </summary>
    public static class LogHelper
    {
        /// <summary>
        /// 输出日志信息更明显
        /// </summary>
        /// <param name="logInfo"></param>
        /// <returns></returns>
        public static string OutputClearness(string logInfo)
        {
            return $"|===---~~~______~~~---===\r\n{(string.IsNullOrEmpty(logInfo) ? "Nothing" : logInfo)}\r\n===---~~~______~~~---===";
        }
    }
}
