using System.Collections.Generic;

namespace PracticeManage.ViewModel.Task
{
    public class TaskNewDto : BaseTaskDto
    {
        public List<long> SubjectList { get; set; }
    }
}
