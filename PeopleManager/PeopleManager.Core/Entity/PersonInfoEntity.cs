using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PeopleManager.Core.Entity
{
    /// <summary>
    /// 人员信息
    /// </summary>
    [Table("t_person_info")]
    public class PersonInfoEntity : BaseEntity
    {
        /// <summary>
        /// 原始表id
        /// </summary>
        [Column("original_id")]
        public long OriginalId { get; set; } = 0;

        /// <summary>
        /// 教员标志
        /// </summary>
        [Column("teacher_flag")]
        public int TeacherFlag { get; set; } = 0;

        /// <summary>
        /// 学员标志
        /// </summary>
        [Column("student_flag")]
        public int StudentFlag { get; set; } = 1;

        /// <summary>
        /// 名字
        /// </summary>
        [Column("user_name")]
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// 工号
        /// </summary>
        [Column("user_number")]
        public string UserNumber { get; set; } = string.Empty;

        /// <summary>
        /// 性别
        /// </summary>
        [Column("gender")]
        public string Gender { get; set; }

        /// <summary>
        /// 生日
        /// </summary>
        [Column("birthday")]
        public DateTime? Birthday { get; set; }

        /// <summary>
        /// 学历key
        /// </summary>
        [Column("education_key")]
        public string EducationKey { get; set; }

        /// <summary>
        /// 学历value
        /// </summary>
        [Column("education_value")]
        public string EducationValue { get; set; }

        /// <summary>
        /// 毕业学校
        /// </summary>
        [Column("school_tag")]
        public string SchoolTag { get; set; }

        /// <summary>
        /// 家庭地址
        /// </summary>
        [Column("house_address")]
        public string HouseAddress { get; set; }

        /// <summary>
        /// 常住地
        /// </summary>
        [Column("regular_address")]
        public string RegularAddress { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        [Column("user_phone")]
        public string UserPhone { get; set; } = string.Empty;

        /// <summary>
        /// 国籍
        /// </summary>
        [Column("nationality")]
        public string Nationality { get; set; }

        /// <summary>
        /// 民族
        /// </summary>
        [Column("nation")]
        public string Nation { get; set; }

        /// <summary>
        /// 血型
        /// </summary>
        [Column("blood_type")]
        public string BloodType { get; set; }

        /// <summary>
        /// 籍贯
        /// </summary>
        [Column("native_place")]
        public string NativePlace { get; set; }

        /// <summary>
        /// 婚姻状况
        /// </summary>
        [Column("marriage_status")]
        public string MarriageStatus { get; set; }

        /// <summary>
        /// 健康状况
        /// </summary>
        [Column("state_of_health")]
        public string StateOfHealth { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        [Column("user_email")]
        public string UserEmail { get; set; }

        /// <summary>
        /// 参加工作日期
        /// </summary>
        [Column("employment_date")]
        public DateTime? EmploymentDate { get; set; }

        /// <summary>
        /// 头像路径
        /// </summary>
        [Column("photo_path")]
        public string PhotoPath { get; set; }

        /// <summary>
        /// 人员密级
        /// </summary>
        [Column("sec_level")]
        public int? SecLevel { get; set; }

        #region ::::: 资质信息 :::::
        /// <summary>
        /// 资质名称
        /// </summary>
        [Display(Name = "资质名称")]
        [Column("qualification_name")]
        public string QualificationName { get; set; }

        /// <summary>
        /// 资质类型Key
        /// </summary>
        [Display(Name = "资质类型Key")]
        [Column("qualification_type_key")]
        public string QualificationTypeKey { get; set; }

        /// <summary>
        /// 资质类型Value
        /// </summary>
        [Display(Name = "资质类型Value")]
        [Column("qualification_type_value")]
        public string QualificationTypeValue { get; set; }

        /// <summary>
        /// 获取时间
        /// </summary>
        [Display(Name = "资质获取时间")]
        [Column("qualification_get_date")]
        public DateTime? QualificationGetDate { get; set; }

        /// <summary>
        /// 到期时间
        /// </summary>
        [Display(Name = "资质到期时间")]
        [Column("qualification_expiration_date")]
        public DateTime? QualificationExpirationDate { get; set; }


        #endregion

        /// <summary>
        /// 工作信息
        /// </summary>
        public IList<WorkInfoEntity> WorkInfos { get; set; } = new List<WorkInfoEntity>();

        /// <summary>
        /// 执照信息
        /// </summary>
        public IList<CertificateInfoEntity> CertificateInfos { get; set; } = new List<CertificateInfoEntity>();

        /// <summary>
        /// 培训记录
        /// </summary>
        public IList<TrainingRecordEntity> TrainingRecords { get; set; } = new List<TrainingRecordEntity>();

        /// <summary>
        /// 证照信息
        /// </summary>
        public IList<LicenseInfoEntity> LicenseInfos { get; set; } = new List<LicenseInfoEntity>();

        /// <summary>
        /// 奖惩记录
        /// </summary>
        public IList<RewardsAndPunishmentEntity> RewardsAndPunishments { get; set; } = new List<RewardsAndPunishmentEntity>();
    }
}
