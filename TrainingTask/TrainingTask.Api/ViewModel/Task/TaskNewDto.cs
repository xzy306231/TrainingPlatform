using System.Collections.Generic;
using Newtonsoft.Json;
using TrainingTask.Api.ViewModel.Ref;

namespace TrainingTask.Api.ViewModel.Task
{
    public class TaskNewDto: TaskBaseDto
    {
        [JsonProperty("taskSubjectRef")]
        public IList<TaskSubjectRefNewDto> SubjectRefEntities { get; set; }
    }
}
