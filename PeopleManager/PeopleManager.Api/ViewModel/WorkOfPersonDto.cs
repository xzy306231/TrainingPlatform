using System;

namespace PeopleManager.Api.ViewModel
{
    public class WorkOfPersonDto
    {
        /// <summary>
        /// 所属单位key
        /// </summary>
        public string DepartmentKey { get; set; }

        /// <summary>
        /// 所属单位value
        /// </summary>
        public string DepartmentValue { get; set; }

        /// <summary>
        /// 教员类型key
        /// </summary>
        public string TeacherTypeKey { get; set; }

        /// <summary>
        /// 教员类型value
        /// </summary>
        public string TeacherTypeValue { get; set; }

        /// <summary>
        /// 机型key
        /// </summary>
        public string AirplaneModelKey { get; set; }

        /// <summary>
        /// 机型value
        /// </summary>
        public string AirplaneModelValue { get; set; }

        /// <summary>
        /// 飞行状态key
        /// </summary>
        public string FlyStatusKey { get; set; }

        /// <summary>
        /// 飞行状态value
        /// </summary>
        public string FlyStatusValue { get; set; }

        /// <summary>
        /// 运行基地key
        /// </summary>
        public string BaseKey { get; set; }

        /// <summary>
        /// 运行基地value
        /// </summary>
        public string BaseValue { get; set; }

        /// <summary>
        /// 技术等级key
        /// </summary>
        public string SkillLevelKey { get; set; }

        /// <summary>
        /// 技术等级value
        /// </summary>
        public string SkillLevelValue { get; set; }

        /// <summary>
        /// 入职日期
        /// </summary>
        public DateTime? HireDate { get; set; }

        #region ::::: 飞行数据 :::::

        /// <summary>
        /// 总飞行时长
        /// </summary>
        public double TotalDuration { get; set; }

        /// <summary>
        /// 总模拟训练时长
        /// </summary>
        public double TrainingDuration { get; set; }

        /// <summary>
        /// 总起落次数
        /// </summary>
        public int ActualFlightNumber { get; set; }

        /// <summary>
        /// 总飞行经历时间
        /// </summary>
        public double ActualDuration { get; set; }

        /// <summary>
        /// 模拟训练起落次数
        /// </summary>
        public double CurrentActualDuration { get; set; }

        /// <summary>
        /// 本机型起落次数
        /// </summary>
        public int CurrentFlightNumber { get; set; }

        #endregion
    }
}
