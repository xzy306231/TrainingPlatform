using System;
using System.ComponentModel.DataAnnotations;

namespace PeopleManager.Api.ViewModel
{
    public class WorkEditInfoDto
    {
        /// <summary>
        /// 所属单位key
        /// </summary>
        [Required]
        public string DepartmentKey { get; set; }

        /// <summary>
        /// 所属单位value
        /// </summary>
        [Required]
        public string DepartmentValue { get; set; }

        /// <summary>
        /// 教员类型key
        /// </summary>
        [Required]
        public string TeacherTypeKey { get; set; }

        /// <summary>
        /// 教员类型value
        /// </summary>
        [Required]
        public string TeacherTypeValue { get; set; }

        /// <summary>
        /// 机型key
        /// </summary>
        [Required]
        public string AirplaneModelKey { get; set; }

        /// <summary>
        /// 机型value
        /// </summary>
        [Required]
        public string AirplaneModelValue { get; set; }

        /// <summary>
        /// 飞行状态key
        /// </summary>
        [Required]
        public string FlyStatusKey { get; set; }

        /// <summary>
        /// 飞行状态value
        /// </summary>
        [Required]
        public string FlyStatusValue { get; set; }

        /// <summary>
        /// 运行基地key
        /// </summary>
        [Required]
        public string BaseKey { get; set; }

        /// <summary>
        /// 运行基地value
        /// </summary>
        [Required]
        public string BaseValue { get; set; }

        /// <summary>
        /// 技术等级key
        /// </summary>
        [Required]
        public string SkillLevelKey { get; set; }

        /// <summary>
        /// 技术等级value
        /// </summary>
        [Required]
        public string SkillLevelValue { get; set; }

        /// <summary>
        /// 入职日期
        /// </summary>
        [Required]
        public DateTime HireDate { get; set; }
    }
}
