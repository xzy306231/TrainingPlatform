using System;
using Newtonsoft.Json;

namespace PracticeManage.ViewModel.Subject
{
    public class SubjectQueryDto : BaseSubjectDto
    {
        /// <summary>
        /// id
        /// </summary>
        [JsonProperty("id")]
        public long Id { get; set; }


        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 创建人id
        /// </summary>
        public virtual long CreatorId { get; set; }

        /// <summary>
        /// 创建人名
        /// </summary>
        public virtual string CreatorName { get; set; }

        /// <summary>
        /// 标签展示
        /// </summary>
        public virtual string TagDisplay { get; set; }
    }
}
