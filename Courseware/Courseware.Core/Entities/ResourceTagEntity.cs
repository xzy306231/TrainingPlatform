using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace Courseware.Core.Entities
{
    /// <summary>
    /// 资源标签关联表
    /// </summary>
    [Table("t_resource_tag_ref")]
    public class ResourceTagEntity : BaseEntity
    {
        /// <summary>
        /// 
        /// </summary>
        [Column("resource_id")]
        public long ResourceId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [IgnoreDataMember]
        public virtual ResourceEntity Resource { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("tag_id")]
        public long TagId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual KnowledgeTagEntity Tag { get; set; }
    }
}
