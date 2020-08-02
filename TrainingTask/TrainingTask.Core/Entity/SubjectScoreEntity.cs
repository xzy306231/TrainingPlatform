using System.ComponentModel.DataAnnotations.Schema;

namespace TrainingTask.Core.Entity
{
    /// <summary>
    /// 科目成绩
    /// </summary>
    [Table("t_subject_score")]
    public class SubjectScoreEntity : BaseEntity
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Column("id")]
        public long Id { get; set; }

        /// <summary>
        /// 任务成绩单id
        /// </summary>
        [Column("task_id")]
        public long TaskId { get; set; }

        /// <summary>
        /// 任务成绩单id
        /// </summary>
        [Column("subject_id")]
        public long SubjectId { get; set; }

        /// <summary>
        /// 科目名称
        /// </summary>
        [Column("subject_name")]
        public string SubjectName { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        [Column("desc")]
        public string Desc { get; set; }

        /// <summary>
        /// 标签展示
        /// </summary>
        [Column("tag_display")]
        public string TagDisplay { get; set; }

        /// <summary>
        /// 适用机型value
        /// </summary>
        [Column("airplane_value")]
        public string AirplaneValue { get; set; }

        /// <summary>
        /// 分类value
        /// </summary>
        [Column("classify_value")]
        public string ClassifyValue { get; set; }

        /// <summary>
        /// 科目结果
        /// </summary>
        [Column("result")]
        public int Result { get; set; }

        /// <summary>
        /// 科目状态
        /// </summary>
        [Column("status")]
        public int Status { get; set; }

        /// <summary>
        /// 任务成绩
        /// </summary>
        public TaskScoreEntity TaskScore { get; set; }
    }
}
