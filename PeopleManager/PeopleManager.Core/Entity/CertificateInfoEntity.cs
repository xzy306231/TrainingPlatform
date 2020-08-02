using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PeopleManager.Core.Entity
{
    /// <summary>
    /// 执照信息
    /// </summary>
    [Table("t_certificate_info")]
    public class CertificateInfoEntity : BaseEntity
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        [Display(Name = "用户Id")]
        [Column("person_id")]
        public long PersonId { get; set; }

        /// <summary>
        /// 执照名称
        /// </summary>
        [Display(Name = "执照名称")]
        [Column("name")]
        public string Name { get; set; }

        /// <summary>
        /// 执照编号
        /// </summary>
        [Display(Name = "执照编号")]
        [Column("code")]
        public string Code { get; set; }

        /// <summary>
        /// 执照类型Key
        /// </summary>
        [Display(Name = "执照类型Key")]
        [Column("type_key")]
        public string TypeKey { get; set; }

        /// <summary>
        /// 机型key
        /// </summary>
        [Display(Name = "机型key")]
        [Column("airplane_model_key")]
        public string AirplaneModelKey { get; set; }

        /// <summary>
        /// 机型value
        /// </summary>
        [Display(Name = "机型value")]
        [Column("airplane_model_value")]
        public string AirplaneModelValue { get; set; }

        /// <summary>
        /// 执照类型Value
        /// </summary>
        [Display(Name = "执照类型Value")]
        [Column("type_value")]
        public string TypeValue { get; set; }

        /// <summary>
        /// 是否有效Key
        /// </summary>
        [Display(Name = "是否有效Key")]
        [Column("valid_key")]
        public string ValidKey { get; set; }

        /// <summary>
        /// 是否有效Value
        /// </summary>
        [Display(Name = "是否有效Value")]
        [Column("valid_value")]
        public string ValidValue { get; set; }

        /// <summary>
        /// 考试通过日期
        /// </summary>
        [Display(Name = "考试通过日期")]
        [Column("pass_date")]
        public DateTime? PassDate { get; set; }

        /// <summary>
        /// 拿证日期
        /// </summary>
        [Display(Name = "拿证日期")]
        [Column("get_date")]
        public DateTime? GetDate { get; set; }

        /// <summary>
        /// 最后签注日期
        /// </summary>
        [Display(Name = "最后签注日期")]
        [Column("last_endorse_date")]
        public DateTime? LastEndorseDate { get; set; }

        /// <summary>
        /// 过期日期
        /// </summary>
        [Display(Name = "过期日期")]
        [Column("expiration_date")]
        public DateTime? ExpirationDate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public PersonInfoEntity PersonInfo { get; set; }
    }
}
