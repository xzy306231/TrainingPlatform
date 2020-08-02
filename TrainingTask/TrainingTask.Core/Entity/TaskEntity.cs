using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrainingTask.Core.Entity
{
    /// <summary>
    /// 训练任务
    /// </summary>
    [Table("t_task")]
    public class TaskEntity : BaseEntity
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Column("id")]
        public long Id { get; set; }

        /// <summary>
        /// 任务名称
        /// </summary>
        [Column("task_name")]
        public string TaskName { get; set; }

        /// <summary>
        /// 任务描述
        /// </summary>
        [Column("task_desc")]
        public string TaskDesc { get; set; }

        /// <summary>
        /// 任务类别key
        /// </summary>
        [Column("task_type_key")]
        public string TaskTypeKey { get; set; }

        /// <summary>
        /// 任务类别value
        /// </summary>
        [Column("task_type_value")]
        public string TaskTypeValue { get; set; }

        /// <summary>
        /// 类别等级key
        /// </summary>
        [Column("type_level_key")]
        public string TypeLevelKey { get; set; }

        /// <summary>
        /// 类别等级value
        /// </summary>
        [Column("type_level_value")]
        public string TypeLevelValue { get; set; }

        /// <summary>
        /// 级别等级key
        /// </summary>
        [Column("level_key")]
        public string LevelKey { get; set; }

        /// <summary>
        /// 级别等级value
        /// </summary>
        [Column("level_value")]
        public string LevelValue { get; set; }

        /// <summary>
        /// 适用机型key
        /// </summary>
        [Column("airplane_type_key")]
        public string AirplaneTypeKey { get; set; }

        /// <summary>
        /// 适用机型value
        /// </summary>
        [Column("airplane_type_value")]
        public string AirplaneTypeValue { get; set; }

        /// <summary>
        /// 课时
        /// </summary>
        [Column("class_hour")]
        public int? ClassHour { get; set; }

        /// <summary>
        /// 创建人id
        /// </summary>
        [Column("creator_id")]
        public long? CreatorId { get; set; }

        /// <summary>
        /// 创建人名
        /// </summary>
        [Column("creator_name")]
        public string CreatorName { get; set; }

        /// <summary>
        /// 标签显示
        /// </summary>
        [Column("tag_display")]
        public string TagDisplay { get; set; }

        /// <summary>
        /// 副本
        /// </summary>
        [Column("copy")]
        public int Copy { get; set; }

        /// <summary>
        /// 完成率
        /// </summary>
        [Column("finish_percent")]
        public float FinishPercent { get; set; }

        /// <summary>
        /// 通过率
        /// </summary>
        [Column("pass_percent")]
        public float PassPercent { get; set; }

        /// <summary>
        /// 平均时长
        /// </summary>
        [Column("duration_avg")]
        public float DurationAvg { get; set; }

        /// <summary>
        /// 任务科目关联表
        /// </summary>
        public IList<TaskSubjectRefEntity> SubjectRefEntities { get; set; } = new List<TaskSubjectRefEntity>();
    }
}
