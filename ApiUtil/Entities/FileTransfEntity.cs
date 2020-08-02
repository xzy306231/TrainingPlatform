using Newtonsoft.Json;

namespace ApiUtil.Entities
{
    /// <summary>
    /// 
    /// </summary>
    public class FileTransfEntity
    {
        /// <summary>
        /// 文件后缀名
        /// </summary>
        [JsonProperty("exeName")]
        public string FileSuffix { get; set; }

        /// <summary>
        /// 原始文件路径
        /// </summary>
        [JsonProperty("filePath")]
        public string OriginalUrl { get; set; }

        /// <summary>
        /// 组名
        /// </summary>
        [JsonProperty("groupName")]
        public string GroupName { get; set; }

        /// <summary>
        /// 资源名
        /// </summary>
        [JsonProperty("resourceName")]
        public string ResourceName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("resourceId")]
        public long SourceId { get; set; }
    }
}
