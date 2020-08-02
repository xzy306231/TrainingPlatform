using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Courseware.Core.Entities
{
    /// <summary>
    /// 
    /// </summary>
    public class BaseEntity
    {
        /// <summary>
        /// 逻辑删除
        /// 状态0-正常，1-逻辑删除
        /// </summary>
        [Column("delete_flag")]
        public int? DeleteFlag { get; set; } = 0;

        /// <summary>
        /// 创建时间
        /// </summary>
        [Column("t_create")]
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        [Column("t_modified")]
        public DateTime ModifiedTime { get; set; }
    }
}
