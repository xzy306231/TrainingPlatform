using System;
using FreeSql.DataAnnotations;

namespace PracticeManage.Entity
{
    /// <summary>
    /// 
    /// </summary>
    public class BaseEntity
    {
        /// <summary>
        /// 逻辑删除
        /// </summary>
        [Column(Name = "delete_flag")]
        public sbyte DeleteFlag { get; set; } = 0;

        /// <summary>
        /// 创建时间
        /// </summary>
        [Column(Name = "t_create", ServerTime = DateTimeKind.Utc, CanUpdate = false)]
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        [Column(Name = "t_modified", ServerTime = DateTimeKind.Utc)]
        public DateTime? ModifiedTime { get; set; }
    }
}
