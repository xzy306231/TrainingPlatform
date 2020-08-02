using System.Collections.Generic;
using TrainingTask.Api.ViewModel.Ref;

namespace TrainingTask.Api.ViewModel.Subject
{
    public class SubjectFullDto : SubjectBaseDto
    {        
        /// <summary>
        /// 科目id
        /// </summary>
        public long Id { get; set; }


        public IList<SubjectTagRefDto> TagRefEntities { get; set; }
    }
}
