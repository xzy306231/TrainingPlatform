using System.Collections.Generic;
using Newtonsoft.Json;

namespace PracticeManage.ViewModel.Subject
{
    public class SubjectCreateDto: BaseSubjectDto
    {

        /// <summary>
        /// 创建人id
        /// </summary>
        public virtual long CreatorId { get; set; }

        /// <summary>
        /// 创建人名
        /// </summary>
        public virtual string CreatorName { get; set; }

        [JsonProperty("tags")]
        public List<TagDto> TagList { get; set; }
    }
}
