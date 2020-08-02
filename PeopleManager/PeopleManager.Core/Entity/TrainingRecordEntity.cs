using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PeopleManager.Core.Entity
{
    /// <summary>
    /// 培训记录
    /// </summary>
    [Table("t_training_record")]
    public class TrainingRecordEntity : BaseEntity
    {
        /// <summary>
        /// 平台中人员id
        /// </summary>
        [Display(Name = "人员id")]
        [Column("person_id")]
        public long PersonId { get; set; }

        /// <summary>
        /// 培训日期
        /// </summary>
        [Display(Name = "培训日期")]
        [Column("training_date")]
        public DateTime? TrainingDate { get; set; }

        /// <summary>
        /// 培训项目
        /// </summary>
        [Display(Name = "培训项目")]
        [Column("project_name")]
        public string ProjectName { get; set; }

        /// <summary>
        /// 培训内容
        /// </summary>
        [Display(Name = "培训内容")]
        [Column("content")]
        public string Content { get; set; }

        /// <summary>
        /// 培训状态key
        /// </summary>
        [Display(Name = "培训状态key")]
        [Column("status_key")]
        public string StatusKey { get; set; }

        /// <summary>
        /// 培训状态value
        /// </summary>
        [Display(Name = "培训状态value")]
        [Column("status_value")]
        public string StatusValue { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public PersonInfoEntity PersonInfo { get; set; }
    }
}
