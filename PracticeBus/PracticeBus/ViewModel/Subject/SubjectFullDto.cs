using System.Collections.Generic;

namespace PracticeBus.ViewModel.Subject
{
    public class SubjectFullDto : BaseSubjectDto
    {
        /// <summary>
        /// 
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public long OriginalId { get; set; }

        public List<TagDto> Tags { get; set; }
    }
}
