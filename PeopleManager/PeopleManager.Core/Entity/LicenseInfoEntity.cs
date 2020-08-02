using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PeopleManager.Core.Entity
{
    /// <summary>
    /// 证照信息
    /// </summary>
    [Table("t_license_info")]
    public class LicenseInfoEntity : BaseEntity
    {
        /// <summary>
        /// 平台中人员id
        /// </summary>
        [Display(Name = "人员id")]
        [Column("person_id")]
        public long PersonId { get; set; }

        /// <summary>
        /// 证照名称
        /// </summary>
        [Display(Name = "证照名称")]
        [Column("license_name")]
        public string LicenseName { get; set; }

        /// <summary>
        /// 是否有效
        /// </summary>
        [Display(Name = "是否有效")]
        [Column("valid_key")]
        public string ValidKey { get; set; }

        /// <summary>
        /// 是否有效
        /// </summary>
        [Display(Name = "是否有效")]
        [Column("valid_value")]
        public string ValidValue { get; set; }

        /// <summary>
        /// 开始日期
        /// </summary>
        [Display(Name = "开始日期")]
        [Column("start_date")]
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// 结束日期
        /// </summary>
        [Display(Name = "结束日期")]
        [Column("end_date")]
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public PersonInfoEntity PersonInfo { get; set; }
    }
}
