using System.Collections.Generic;

namespace PracticeManage.ViewModel.Subject
{
    public class SubjectBusFullDto : SubjectBusNewDto
    {
        public long Id { get; set; }

        public List<TagDto> Tags { get; set; }
    }
}
