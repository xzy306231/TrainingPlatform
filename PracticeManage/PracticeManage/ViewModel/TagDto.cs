using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace PracticeManage.ViewModel
{
    public class TagDto
    {
        /// <summary>
        /// 知识点原始id
        /// </summary>
        [Required]
        [JsonProperty("tagId")]
        public long OriginalId { get; set; }

        /// <summary>
        /// 知识点名称
        /// </summary>
        [Display(Name = "知识点"), Required]
        public string TagName { get; set; }
    }
}
