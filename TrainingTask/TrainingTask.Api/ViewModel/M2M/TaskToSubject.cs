using System.Collections.Generic;
using TrainingTask.Core.Entity;

namespace TrainingTask.Api.ViewModel.M2M
{
    public class TaskToSubject
    {
        public TaskEntity Task { get; set; }

        public IList<long> SubjectIds { get; set; }
    }
}
