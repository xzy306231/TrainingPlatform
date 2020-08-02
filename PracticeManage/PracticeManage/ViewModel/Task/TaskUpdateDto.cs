using System.Collections.Generic;

namespace PracticeManage.ViewModel.Task
{
    public class TaskUpdateDto : BaseTaskDto
    {
        /// <summary>
        /// 
        /// </summary>
        public long Id { get; set; }

        public List<long> SubjectList { get; set; }
    }
}
