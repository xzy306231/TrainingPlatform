namespace TrainingTask.Api.ViewModel.Task
{
    public class TaskStatistics
    {
        /// <summary>
        /// 任务名称
        /// </summary>
        public string TaskName { get; set; }

        /// <summary>
        /// 完成率
        /// </summary>
        public float FinishPercent { get; set; }

        /// <summary>
        /// 通过率
        /// </summary>
        public float PassPercent { get; set; }

        /// <summary>
        /// 平均时长
        /// </summary>
        public float DurationAvg { get; set; }
    }
}
