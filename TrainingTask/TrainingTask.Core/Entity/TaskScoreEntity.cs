using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrainingTask.Core.Entity
{
    /// <summary>
    /// 训练任务成绩
    /// </summary>
    [Table("t_task_score")]
    public class TaskScoreEntity : BaseEntity
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Column("id")]
        public long Id { get; set; }

        /// <summary>
        /// 用户id
        /// </summary>
        [Column("userId")]
        public long UserId { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        [Column("userName")]
        public string UserName { get; set; }

        /// <summary>
        /// 所属部门
        /// </summary>
        [Column("department")]
        public string Department { get; set; }

        /// <summary>
        /// 计划id
        /// </summary>
        [Column("plan_id")]
        public long PlanId { get; set; }

        /// <summary>
        /// 任务id
        /// </summary>
        [Column("task_id")]
        public long TaskId { get; set; }

        /// <summary>
        /// 任务名称
        /// </summary>
        [Column("task_name")]
        public string TaskName { get; set; }

        /// <summary>
        /// 任务结果
        /// 默认为1 未通过
        /// </summary>
        [Column("result")]
        public int Result { get; set; } = 1;

        /// <summary>
        /// 任务状态
        /// 默认为1 未完成
        /// </summary>
        [Column("status")]
        public int Status { get; set; } = 1;

        /// <summary>
        /// 开始时间
        /// </summary>
        [Column("start_time")]
        public DateTime? StartTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        [Column("end_time")]
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// 持续时长
        /// </summary>
        [Column("duration")]
        public float Duration { get; set; }

        /// <summary>
        /// 科目成绩
        /// </summary>
        public IList<SubjectScoreEntity> SubjectScores { get; set; } = new List<SubjectScoreEntity>();

    }
}
