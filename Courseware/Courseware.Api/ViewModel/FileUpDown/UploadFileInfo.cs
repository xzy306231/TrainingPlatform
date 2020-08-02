using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Courseware.Api.ViewModel.FileUpDown
{
    /// <summary>
    /// 上传的课件文件信息
    /// </summary>
    public class UploadFileInfo
    {
        [JsonProperty("fastUrl")]
        public string FastUrl { get; set; }

        /// <summary>
        /// 原始文件路径
        /// </summary>
        [JsonProperty("filePath")]
        public string OriginalUrl { get; set; }

        [JsonProperty("fullPath")]
        public string FullPath { get; set; }

        /// <summary>
        /// 文件后缀名
        /// </summary>
        [JsonProperty("exeName")]
        public string FileSuffix { get; set; }

        /// <summary>
        /// 文件大小
        /// </summary>
        [Required]
        public int FileSize { get; set; }

        /// <summary>
        /// 组名
        /// </summary>
        [JsonProperty("groupName")]
        public string GroupName { get; set; }

        /// <summary>
        /// 资源时长
        /// </summary>
        public int ResourceDuration { get; set; }

        /// <summary>
        /// 资源名
        /// </summary>
        [Required]
        public string ResourceName { get; set; }

        /// <summary>
        /// 资源类型
        /// </summary>
        public string ResourceType { get; set; }

        /// <summary>
        /// 资源文件大小--显示用
        /// </summary>
        [JsonProperty("showFileSize")]
        public string FileSizeDisplay { get; set; }

        /// <summary>
        /// 创建人id
        /// </summary>
        [Required]
        public long CreatorId { get; set; }

        /// <summary>
        /// 创建人名
        /// </summary>
        [Required]
        public string CreatorName { get; set; }

        /// <summary>
        /// 资源密级
        /// </summary>
        public string ResourceLevel { get; set; }
    }
}
