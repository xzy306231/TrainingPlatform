using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace PracticeBus.ViewModel
{
    public class TagDto
    {
        /// <summary>
        /// 知识点原始id
        /// </summary>
        [Required]
        [JsonProperty("tagId")]
        public long SrcId { get; set; }

        /// <summary>
        /// 知识点名称
        /// </summary>
        [Display(Name = "知识点"), Required]
        [JsonProperty("tagName")]
        public string Tag { get; set; }
    }
}
