using System.Collections.Generic;
using Newtonsoft.Json;
using PracticeBus.ViewModel.Subject;

namespace PracticeBus.ViewModel.Task
{
    public class TaskNewDto : BaseTaskDto
    {
        /// <summary>
        /// 主键
        /// </summary>
        [JsonProperty("Id")]
        public long OriginalId { get; set; }

        public List<SubjectNewDto> Subjects { get; set; }
    }
}
