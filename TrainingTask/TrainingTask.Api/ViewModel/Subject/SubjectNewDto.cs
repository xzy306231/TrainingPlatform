using System.Collections.Generic;
using Newtonsoft.Json;
using TrainingTask.Api.ViewModel.Ref;

namespace TrainingTask.Api.ViewModel.Subject
{
    /// <summary>
    /// 科目
    /// </summary>
    public class SubjectNewDto : SubjectBaseDto
    {
        [JsonProperty("subjectTags")]
        public IList<SubjectTagRefDto> TagRefEntities { get; set; }
    }
}
