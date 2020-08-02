using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Courseware.Core.Entities
{
    /// <summary>
    /// 标签
    /// </summary>
    [Table("t_knowledge_tag")]
    public class KnowledgeTagEntity : BaseEntity
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
        /// 资源关联表
        /// </summary>
        public virtual ICollection<ResourceTagEntity> ResourceTags { get; set; } = new List<ResourceTagEntity>();
    }
}
