using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PeopleManager.Api.ViewModel
{
    public class PersonEditDto
    {
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
        [Required]
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
        /// 工作信息
        /// </summary>
        public IList<WorkEditInfoDto> WorkInfos { get; set; } = new List<WorkEditInfoDto>();
    }
}
