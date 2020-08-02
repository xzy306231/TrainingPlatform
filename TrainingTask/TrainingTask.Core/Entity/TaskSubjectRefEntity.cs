using System.ComponentModel.DataAnnotations.Schema;

namespace TrainingTask.Core.Entity
{
    /// <summary>
    /// 任务科目关联表
    /// </summary>
    [Table("t_task_subject_ref")]
    public class TaskSubjectRefEntity : BaseEntity
    {
        /// <summary>
        /// 任务id
        /// </summary>
        [Column("task_id")]
        public long TaskId { get; set; }

        /// <summary>
        /// 任务
        /// </summary>
        public TaskEntity Task { get; set; }

        /// <summary>
        /// 科目id
        /// </summary>
        [Column("subject_id")]
        public long SubjectId { get; set; }

        /// <summary>
        /// 科目
        /// </summary>
        public SubjectEntity Subject { get; set; }
    }
}
