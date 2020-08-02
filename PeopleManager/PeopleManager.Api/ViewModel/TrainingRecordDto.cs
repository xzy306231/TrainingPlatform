using System;

namespace PeopleManager.Api.ViewModel
{
    public class TrainingRecordDto
    {
        /// <summary>
        /// 培训日期
        /// </summary>
        public DateTime? TrainingDate { get; set; }

        /// <summary>
        /// 培训项目
        /// </summary>
        public string ProjectName { get; set; }

        /// <summary>
        /// 培训内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 培训状态key
        /// </summary>
        public string StatusKey { get; set; }

        /// <summary>
        /// 培训状态value
        /// </summary>
        public string StatusValue { get; set; }
    }
}
