using System;
using System.Collections.Generic;

namespace PeopleManager.Api.ViewModel
{
    public class SinglePersonInfoDto
    {
        /// <summary>
        /// 主键
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 原始表id
        /// </summary>
        public long OriginalId { get; set; }

        /// <summary>
        /// 教员标志
        /// </summary>
        public int TeacherFlag { get; set; }

        /// <summary>
        /// 学员标志
        /// </summary>
        public int StudentFlag { get; set; }

        /// <summary>
        /// 名字
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 工号
        /// </summary>
        public string UserNumber { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        public string Gender { get; set; }

        /// <summary>
        /// 生日
        /// </summary>
        public DateTime? Birthday { get; set; }

        /// <summary>
        /// 学历key
        /// </summary>
        public string EducationKey { get; set; }

        /// <summary>
        /// 学历value
        /// </summary>
        public string EducationValue { get; set; }

        /// <summary>
        /// 毕业学校
        /// </summary>
        public string SchoolTag { get; set; }

        /// <summary>
        /// 家庭地址
        /// </summary>
        public string HouseAddress { get; set; }

        /// <summary>
        /// 常住地
        /// </summary>
        public string RegularAddress { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        public string UserPhone { get; set; }

        /// <summary>
        /// 国籍
        /// </summary>
        public string Nationality { get; set; }

        /// <summary>
        /// 民族
        /// </summary>
        public string Nation { get; set; }

        /// <summary>
        /// 血型
        /// </summary>
        public string BloodType { get; set; }

        /// <summary>
        /// 籍贯
        /// </summary>
        public string NativePlace { get; set; }

        /// <summary>
        /// 婚姻状况
        /// </summary>
        public string MarriageStatus { get; set; }

        /// <summary>
        /// 健康状况
        /// </summary>
        public string StateOfHealth { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        public string UserEmail { get; set; }

        /// <summary>
        /// 参加工作日期
        /// </summary>
        public DateTime? EmploymentDate { get; set; }

        /// <summary>
        /// 头像
        /// </summary>
        public string PhotoPath { get; set; }

        /// <summary>
        /// 人员密级
        /// </summary>
        public int SecLevel { get; set; }

        #region ::::: 资质信息 :::::
        /// <summary>
        /// 资质名称
        /// </summary>
        public string QualificationName { get; set; }

        /// <summary>
        /// 资质类型Key
        /// </summary>
        public string QualificationTypeKey { get; set; }

        /// <summary>
        /// 资质类型Value
        /// </summary>
        public string QualificationTypeValue { get; set; }

        /// <summary>
        /// 获取时间
        /// </summary>
        public DateTime? QualificationGetDate { get; set; }

        /// <summary>
        /// 到期时间
        /// </summary>
        public DateTime? QualificationExpirationDate { get; set; }


        #endregion

        /// <summary>
        /// 工作信息
        /// </summary>
        public IList<WorkOfPersonDto> WorkInfos { get; set; } = new List<WorkOfPersonDto>();

        /// <summary>
        /// 执照信息
        /// </summary>
        public IList<CertificateInfoDto> CertificateInfos { get; set; } = new List<CertificateInfoDto>();

        /// <summary>
        /// 培训记录
        /// </summary>
        public IList<TrainingRecordDto> TrainingRecords { get; set; } = new List<TrainingRecordDto>();

        /// <summary>
        /// 证照信息
        /// </summary>
        public IList<LicenseInfoDto> LicenseInfos { get; set; } = new List<LicenseInfoDto>();

        /// <summary>
        /// 奖惩记录
        /// </summary>
        public IList<RewardsAndPunishmentDto> RewardsAndPunishments { get; set; } = new List<RewardsAndPunishmentDto>();
    }
}
