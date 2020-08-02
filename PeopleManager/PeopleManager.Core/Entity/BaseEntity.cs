using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PeopleManager.Core.Entity
{
    public class BaseEntity
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Display(Name = "主键")]
        [Column("id")]
        public long Id { get; set; }

        /// <summary>
        /// 逻辑删除
        /// </summary>
        [Display(Name = "状态0-正常，1-逻辑删除")]
        [Column("delete_flag")]
        public int? DeleteFlag { get; set; } = 0;

        /// <summary>
        /// 创建时间
        /// </summary>
        [Display(Name = "创建时间")]
        [Column("t_create")]
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        [Display(Name = "修改时间")]
        [Column("t_modified")]
        public DateTime ModifiedTime { get; set; }
    }
}
