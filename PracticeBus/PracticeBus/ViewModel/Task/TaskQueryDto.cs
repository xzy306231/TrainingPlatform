using System.Collections.Generic;
using PracticeBus.ViewModel.Subject;

namespace PracticeBus.ViewModel.Task
{
    public class TaskQueryDto : BaseTaskDto
    {
        /// <summary>
        /// 
        /// </summary>
        public long Id { get; set; }

        public List<SubjectFullDto> Subjects { get; set; }
    }
}
