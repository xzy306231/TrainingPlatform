using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Courseware.Api.ViewModel.FileUpDown
{
    /// <summary>
    /// 课件转换完成
    /// </summary>
    public class TransformComplete
    {
        [Required]
        [JsonProperty("resourceId")]
        public long ResourceId { get; set; }

        [Required]
        [JsonProperty("filePath")]
        public string TransFilePath { get; set; }
    }
}
