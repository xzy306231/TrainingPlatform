namespace TrainingTask.Api.ViewModel.TaskScore
{
    public class MyTaskContentDto
    {
        /// <summary>
        /// 任务名称
        /// </summary>
        public string TaskName { get; set; }

        /// <summary>
        /// 任务描述
        /// </summary>
        public string TaskDesc { get; set; }

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

        /// <summary>
        /// 课时
        /// </summary>
        public int ClassHour { get; set; }
    }
}
