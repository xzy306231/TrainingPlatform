namespace ApiUtil.Log
{
    /// <summary>
    /// 
    /// </summary>
    public interface ILog
    {
        /// <summary>
        /// 
        /// </summary>
        void Information(string message);
        
        /// <summary>
        /// 
        /// </summary>
        void Warning(string message);

        /// <summary>
        /// 
        /// </summary>
        void Debug(string message);

        /// <summary>
        /// 
        /// </summary>
        void Error(string message);
    }
}
