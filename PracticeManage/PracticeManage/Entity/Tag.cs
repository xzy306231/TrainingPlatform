using System.Collections.Generic;
using FreeSql.DataAnnotations;

namespace PracticeManage.Entity
{
    /// <summary>
    /// 知识点表
    /// </summary>
    [Table(Name = "t_tag")]
    public class Tag : BaseEntity
    {
        /// <summary>
        /// 自增主键
        /// </summary>
        [Column(IsIdentity = true, IsPrimary = true,Name = "id")]
        public long Id { get; set; }

        /// <summary>
        /// 知识点原始id
        /// </summary>
        [Column(Name = "original_id")]
        public long OriginalId { get; set; }

        /// <summary>
        /// 知识点名称
        /// </summary>
        [Column(Name = "tag", StringLength = 50)]
        public string TagName { get; set; } = string.Empty;

        /// <summary>
        /// 科目业务表关联
        /// </summary>
        [Navigate(ManyToMany = typeof(SubjectBusTagRefEntity))]
        public virtual List<SubjectBusEntity> SubjectBuses { get; set; }

        /// <summary>
        /// 科目资源表关联
        /// </summary>
        [Navigate(ManyToMany = typeof(SubjectTagRefEntity))]
        public virtual List<Subject> Subjects { get; set; }
    }
}
