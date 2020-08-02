namespace PeopleManager.Api.ViewModel.Server
{
    public class TrainingPlanShowDto
    {
        /// <summary>
        /// 工号
        /// </summary>
        public string UserNumber { get; set; }

        /// <summary>
        /// 名字
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 所属部门
        /// </summary>
        public string DepartmentValue { get; set; }

        /// <summary>
        /// 机型
        /// </summary>
        public string AirplaneModelValue { get; set; }

        /// <summary>
        /// 技术等级
        /// </summary>
        public string SkillLevelValue { get; set; }

        /// <summary>
        /// 飞行时长
        /// </summary>
        public double TotalDuration { get; set; }

        /// <summary>
        /// 飞行状态
        /// </summary>
        public string FlyStatusValue { get; set; }

        /// <summary>
        /// 人员密级
        /// </summary>
        public int SecLevel { get; set; }
    }
}
