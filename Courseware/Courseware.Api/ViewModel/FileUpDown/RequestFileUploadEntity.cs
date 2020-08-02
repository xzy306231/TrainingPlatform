namespace Courseware.Api.ViewModel.FileUpDown
{
    /// <summary>
    /// 文件请求上传实体
    /// </summary>
    public class RequestFileUploadEntity
    {
        /// <summary>
        /// 文件大小
        /// </summary>
        public long Size { get; set; }

        /// <summary>
        /// 片段数量
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// 文件MD5
        /// </summary>
        public string FileData { get; set; }

        /// <summary>
        /// 文件类型
        /// </summary>
        public string FileExt { get; set; }

        ///// <summary>
        ///// 工厂id
        ///// </summary>
        //public string factoryid  { get; set; }

        /// <summary>
        /// 文件名
        /// </summary>
        public string Filename { get; set; }
    }
}
