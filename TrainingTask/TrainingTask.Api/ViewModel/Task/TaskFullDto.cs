using System.Collections.Generic;
using TrainingTask.Api.ViewModel.Ref;

namespace TrainingTask.Api.ViewModel.Task
{
    /// <summary>
    /// 
    /// </summary>
    public class TaskFullDto : TaskBaseDto
    {
        /// <summary>
        /// 任务id
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public IList<TaskSubjectRefUpdateDto> SubjectRefEntities { get; set; }
    }
}
