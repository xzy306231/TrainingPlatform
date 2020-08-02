using System.Collections.Generic;
using Newtonsoft.Json;
using PracticeManage.ViewModel.Subject;

namespace PracticeManage.ViewModel.Task
{
    public class TaskFullDto:BaseTaskDto
    {
        /// <summary>
        /// 主键
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 标签显示
        /// </summary>
        public string TagDisplay { get; set; }

        [JsonProperty("subjects")]
        public List<SubjectBusFullDto> SubjectBusEntities { get; set; }
    }
}
