using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Courseware.Api.ViewModel.Manage
{
    /// <summary>
    /// 知识点内容
    /// </summary>
    public class KnowledgeTagDto
    {
        [Required]
        [JsonProperty("tagId")]
        public long OriginalId { get; set; }

        [Display(Name = "知识点"),Required]
        public string TagName { get; set; }
    }
}
