using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrainingTask.Core.Entity
{
    /// <summary>
    /// 
    /// </summary>
    [Table("t_tag")]
    public class TagEntity : BaseEntity
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Column("id")]
        public long Id { get; set; }

        /// <summary>
        /// 原始表中标签id
        /// </summary>
        [Column("original_id")]
        public long OriginalId { get; set; }

        /// <summary>
        /// 标签名
        /// </summary>
        [Column("tag")]
        public string TagName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public IList<SubjectTagRefEntity> SubjectEntities { get; set; } = new List<SubjectTagRefEntity>();
    }
}
