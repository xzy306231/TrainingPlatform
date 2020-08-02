using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PeopleManager.Core.Entity
{
    [Table("t_work_info")]
    public class WorkInfoEntity : BaseEntity
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        [Display(Name = "用户Id")]
        [Column("person_id")]
        public long PersonId { get; set; }

        /// <summary>
        /// 所属单位key
        /// </summary>
        [Display(Name = "所属单位key")]
        [Column("department_key")]
        public string DepartmentKey { get; set; }

        /// <summary>
        /// 所属单位value
        /// </summary>
        [Display(Name = "所属单位value")]
        [Column("department_value")]
        public string DepartmentValue { get; set; }

        /// <summary>
        /// 教员类型key
        /// </summary>
        [Display(Name = "教员类型key")]
        [Column("teacher_type_key")]
        public string TeacherTypeKey { get; set; }

        /// <summary>
        /// 教员类型value
        /// </summary>
        [Display(Name = "教员类型value")]
        [Column("teacher_type_value")]
        public string TeacherTypeValue { get; set; }

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
        /// 运行基地key
        /// </summary>
        [Display(Name = "运行基地key")]
        [Column("base_key")]
        public string BaseKey { get; set; }

        /// <summary>
        /// 运行基地value
        /// </summary>
        [Display(Name = "运行基地value")]
        [Column("base_value")]
        public string BaseValue { get; set; }

        /// <summary>
        /// 入职日期
        /// </summary>
        [Display(Name = "入职日期")]
        [Column("hire_date")]
        public DateTime? HireDate { get; set; }

        /// <summary>
        /// 技术等级key
        /// </summary>
        [Display(Name = "技术等级key")]
        [Column("skill_level_key")]
        public string SkillLevelKey { get; set; }

        /// <summary>
        /// 技术等级value
        /// </summary>
        [Display(Name = "技术等级value")]
        [Column("skill_level_value")]
        public string SkillLevelValue { get; set; }

        /// <summary>
        /// 飞行状态key
        /// </summary>
        [Display(Name = "飞行状态key")]
        [Column("fly_status_key")]
        public string FlyStatusKey { get; set; }

        /// <summary>
        /// 飞行状态value
        /// </summary>
        [Display(Name = "飞行状态value")]
        [Column("fly_status_value")]
        public string FlyStatusValue { get; set; }

        #region ::::: 飞行数据 :::::

        /// <summary>
        /// 总飞行时长
        /// </summary>
        [Display(Name = "总飞行时长")]
        [Column("total_duration")]
        public double TotalDuration { get; set; }

        /// <summary>
        /// 总模拟训练时长
        /// </summary>
        [Display(Name = "总模拟训练时长")]
        [Column("training_duration")]
        public double TrainingDuration { get; set; }

        /// <summary>
        /// 总起落次数
        /// </summary>
        [Display(Name = "总起落次数")]
        [Column("actual_flight_number")]
        public int ActualFlightNumber { get; set; }

        /// <summary>
        /// 总飞行经历时间
        /// </summary>
        [Display(Name = "实际飞行时长")]
        [Column("actual_duration")]
        public double ActualDuration { get; set; }

        /// <summary>
        /// 模拟训练起落次数
        /// </summary>
        [Display(Name = "本机型总经历时间")]
        [Column("current_actual_duration")]
        public double CurrentActualDuration { get; set; }

        /// <summary>
        /// 本机型起落次数
        /// </summary>
        [Display(Name = "本机型起落次数")]
        [Column("current_flight_number")]
        public int CurrentFlightNumber { get; set; }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        public PersonInfoEntity PersonInfo { get; set; }
    }
}
