using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrainingTask.Core.Entity
{
    /// <summary>
    /// 训练任务科目一对多
    /// </summary>
    [Table("t_subject")]
    public class SubjectEntity : BaseEntity
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Column("id")]
        public long Id { get; set; }

        /// <summary>
        /// 任务id
        /// </summary>
        [Column("task_id")]
        public long TaskId { get; set; }

        /// <summary>
        /// 原始表id
        /// </summary>
        [Column("original_id")]
        public long OriginalId { get; set; }

        /// <summary>
        /// 编号
        /// </summary>
        [Column("subjectNumb")]
        public string SubjectNumb { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [Column("name")]
        public string Name { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        [Column("desc")]
        public string Desc { get; set; }

        /// <summary>
        /// 分类key
        /// </summary>
        [Column("classify_key")]
        public string ClassifyKey { get; set; }

        /// <summary>
        /// 分类value
        /// </summary>
        [Column("classify_value")]
        public string ClassifyValue { get; set; }

        /// <summary>
        /// 适用机型key
        /// </summary>
        [Column("airplane_key")]
        public string AirplaneKey { get; set; }

        /// <summary>
        /// 适用机型value
        /// </summary>
        [Column("airplane_value")]
        public string AirplaneValue { get; set; }

        /// <summary>
        /// 预期结果
        /// </summary>
        [Column("expected_result")]
        public string ExpectedResult { get; set; }

        /// <summary>
        /// 标签展示
        /// </summary>
        [Column("tag_display")]
        public string TagDisplay { get; set; }

        /// <summary>
        /// 副本
        /// </summary>
        [Column("copy")]
        public int Copy { get; set; }

        /// <summary>
        /// 创建人名
        /// </summary>
        [Column("creator_name")]
        public string CreatorName { get; set; }

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
        /// 任务科目关联表
        /// </summary>
        public virtual IList<TaskSubjectRefEntity> TaskRefEntities { get; set; } = new List<TaskSubjectRefEntity>();

        /// <summary>
        /// 知识点关联表
        /// </summary>
        public IList<SubjectTagRefEntity> TagRefEntities { get; set; } = new List<SubjectTagRefEntity>();
    }
}
