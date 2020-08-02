using System.ComponentModel.DataAnnotations;

namespace PracticeManage.ViewModel.Task
{
    public class BaseTaskDto
    {
        /// <summary>
        /// 任务名称
        /// </summary>
        [Required]
        public string TaskName { get; set; }

        /// <summary>
        /// 任务描述
        /// </summary>
        public string TaskDesc { get; set; }

        /// <summary>
        /// 任务类别key
        /// </summary>
        [Required]
        public string TaskTypeKey { get; set; }

        /// <summary>
        /// 类别等级key
        /// </summary>
        [Required]
        public string TypeLevelKey { get; set; }

        /// <summary>
        /// 级别等级key
        /// </summary>
        [Required]
        public string LevelKey { get; set; }

        /// <summary>
        /// 适用机型key
        /// </summary>
        [Required]
        public string AirplaneTypeKey { get; set; }

        /// <summary>
        /// 课时
        /// </summary>
        [Required]
        public int ClassHour { get; set; }

        /// <summary>
        /// 创建人id
        /// </summary>
        public long CreatorId { get; set; }

        /// <summary>
        /// 创建人名
        /// </summary>
        public string CreatorName { get; set; }

        /// <summary>
        /// 任务类别value
        /// </summary>
        public string TaskTypeValue { get; set; }

        /// <summary>
        /// 类别等级value
        /// </summary>
        public string TypeLevelValue { get; set; }

        /// <summary>
        /// 级别等级value
        /// </summary>
        public string LevelValue { get; set; }

        /// <summary>
        /// 适用机型value
        /// </summary>
        public string AirplaneTypeValue { get; set; }
    }
}
