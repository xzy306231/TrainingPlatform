using System.Collections.Generic;
using FreeSql.DataAnnotations;

namespace PracticeBus.Entity
{
    public partial class TTaskBusScore
    {
        /// <summary>
        /// 一对多
        /// 任务成绩对应科目成绩
        /// </summary>
        [Navigate("TaskScoreId")]
        public virtual List<TSubjectBusScore> SubjectBusScores { get; set; }

        /// <summary>
        /// 一对一
        /// 任务成绩和任务
        /// </summary>
        public TTaskBus TaskBus { get; set; }
    }
}
