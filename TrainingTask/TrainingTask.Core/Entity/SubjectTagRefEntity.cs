using System.ComponentModel.DataAnnotations.Schema;

namespace TrainingTask.Core.Entity
{
    /// <summary>
    /// 科目知识点关联表
    /// </summary>
    [Table("t_subject_tag_ref")]
    public class SubjectTagRefEntity : BaseEntity
    {
        /// <summary>
        /// 科目id
        /// </summary>
        [Column("subject_id")]
        public long SubjectId { get; set; }

        /// <summary>
        /// 科目
        /// </summary>
        public SubjectEntity Subject { get; set; }

        /// <summary>
        /// 知识点id
        /// </summary>
        [Column("tag_id")]
        public long TagId { get; set; }

        /// <summary>
        /// 知识点
        /// </summary>
        public TagEntity Tag { get; set; }

    }
}
