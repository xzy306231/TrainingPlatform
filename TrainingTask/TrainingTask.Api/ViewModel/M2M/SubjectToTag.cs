using System.Collections.Generic;
using TrainingTask.Core.Entity;

namespace TrainingTask.Api.ViewModel.M2M
{
    public class SubjectToTag
    {
        public SubjectEntity Subject { get; set; }

        public IList<long> Tags { get; set; }
    }
}
