using System.Collections.Generic;
using FreeSql.DataAnnotations;

namespace PracticeManage.Entity
{
    [Table(Name = "t_subject_bus")]
    public class SubjectBusEntity : BaseSubjectEntity
    {
        /// <summary>
        /// 原始表id
        /// </summary>
        [Column(Name = "original_id")]
        public long OriginalId { get; set; }

        /// <summary>
        /// 任务id
        /// </summary>
        [Column(Name = "task_id")]
        public long TaskId { get; set; }

        /// <summary>
        /// 任务关联对象
        /// </summary>
        public TaskEntity Task { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Navigate(ManyToMany = typeof(SubjectBusTagRefEntity))]
        public virtual List<TagEntity> Tags { get; set; }
    }
}
