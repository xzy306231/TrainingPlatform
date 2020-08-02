using System.Collections.Generic;
using FreeSql.DataAnnotations;

namespace PracticeBus.Entity
{
    public partial class TTaskBus
    {
        [Navigate("TaskBusId")]
        public virtual List<TSubjectBus> Subjects { get; set; }

        [Navigate("TaskBusId")]
        public virtual List<TTaskBusScore> TaskBusScores { get; set; }

        [Navigate("TaskBusId")]
        public virtual List<TSubjectBusScore> SubjectBusScores { get; set; }
    }
}
