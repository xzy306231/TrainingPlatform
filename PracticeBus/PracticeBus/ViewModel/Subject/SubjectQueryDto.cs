using System.Collections.Generic;

namespace PracticeBus.ViewModel.Subject
{
    public class SubjectQueryDto : BaseSubjectDto
    {
        public List<TagDto> Tags { get; set; }
    }
}
