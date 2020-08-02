using System.Collections.Generic;
using FreeSql.DataAnnotations;

namespace PracticeManage.Entity
{
    [Table(Name = "t_task")]
    public class TaskEntity : BaseEntity
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Column(IsIdentity = true,Name = "id")]
        public long Id { get; set; }

        /// <summary>
        /// 任务名称
        /// </summary>
        [Column(Name = "task_name", StringLength = 200)]
        public string TaskName { get; set; }

        /// <summary>
        /// 任务描述
        /// </summary>
        [Column(Name = "task_desc", StringLength = -1)]
        public string TaskDesc { get; set; }

        /// <summary>
        /// 任务类别key
        /// </summary>
        [Column(Name = "task_type_key", StringLength = 20)]
        public string TaskTypeKey { get; set; }

        /// <summary>
        /// 任务类别value
        /// </summary>
        [Column(Name = "task_type_value", StringLength = 50)]
        public string TaskTypeValue { get; set; }

        /// <summary>
        /// 类别等级key
        /// </summary>
        [Column(Name = "type_level_key", StringLength = 20)]
        public string TypeLevelKey { get; set; }

        /// <summary>
        /// 类别等级value
        /// </summary>
        [Column(Name = "type_level_value", StringLength = 50)]
        public string TypeLevelValue { get; set; }

        /// <summary>
        /// 级别等级key
        /// </summary>
        [Column(Name = "level_key", StringLength = 20)]
        public string LevelKey { get; set; }

        /// <summary>
        /// 级别等级value
        /// </summary>
        [Column(Name = "level_value", StringLength = 50)]
        public string LevelValue { get; set; }

        /// <summary>
        /// 适用机型key
        /// </summary>
        [Column(Name = "airplane_type_key", StringLength = 20)]
        public string AirplaneTypeKey { get; set; }

        /// <summary>
        /// 适用机型value
        /// </summary>
        [Column(Name = "airplane_type_value", StringLength = 50)]
        public string AirplaneTypeValue { get; set; }

        /// <summary>
        /// 课时
        /// </summary>
        [Column(Name = "class_hour")]
        public int ClassHour { get; set; }

        /// <summary>
        /// 创建人id
        /// </summary>
        [Column(Name = "creator_id")]
        public long CreatorId { get; set; } = 0;

        /// <summary>
        /// 创建人名
        /// </summary>
        [Column(Name = "creator_name")]
        public string CreatorName { get; set; }

        /// <summary>
        /// 标签显示
        /// </summary>
        [Column(Name = "tag_display")]
        public string TagDisplay { get; set; }

        public virtual List<SubjectBusEntity> SubjectBusEntities { get; set; }
    }
}
