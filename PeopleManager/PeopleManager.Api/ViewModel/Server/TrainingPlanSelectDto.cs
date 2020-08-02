using Newtonsoft.Json;

namespace PeopleManager.Api.ViewModel.Server
{
    public class TrainingPlanSelectDto
    {
        /// <summary>
        /// id
        /// </summary>
        [JsonProperty("id")]
        public long OriginalId { get; set; }

        /// <summary>
        /// 工号
        /// </summary>
        public string UserNumber { get; set; }

        /// <summary>
        /// 名字
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 学历
        /// </summary>
        public string EducationKey { get; set; }

        /// <summary>
        /// 所属部门
        /// </summary>
        public string DepartmentKey { get; set; }

        /// <summary>
        /// 机型
        /// </summary>
        public string AirplaneModelKey { get; set; }

        /// <summary>
        /// 用户头像
        /// </summary>
        public string PhotoPath { get; set; }

        /// <summary>
        /// 技术等级
        /// </summary>
        public string SkillLevelKey { get; set; }

        /// <summary>
        /// 飞行时长
        /// </summary>
        public double TotalDuration { get; set; }

        /// <summary>
        /// 飞行状态
        /// </summary>
        public string FlyStatusKey { get; set; }

        /// <summary>
        /// 人员密级
        /// </summary>
        public int SecLevel { get; set; }
    }
}
