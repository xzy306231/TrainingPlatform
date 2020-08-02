using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PracticeManage.ViewModel.Subject
{
    public class SubjectEditDto : BaseSubjectDto
    {
        public long Id { get; set; }

        /// <summary>
        /// 版本
        /// </summary>
        public int Version { get; set; }


        public List<TagDto> Tags { get; set; }
    }
}
