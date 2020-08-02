namespace PracticeBus.ViewModel.TaskStatistics
{
    public class TaskBusStatisticsQueryDto
    {
        /// <summary>
        /// 任务名
        /// </summary>
        public string TaskName { get; set; }

        /// <summary>
        /// 业务平均时长
        /// </summary>
        public decimal? DurationAvg { get; set; }

        /// <summary>
        /// 业务平均完成率
        /// </summary>
        public decimal? FinishPercent { get; set; }

        /// <summary>
        /// 业务平均通过率
        /// </summary>
        public decimal? PassPercent { get; set; }
    }
}
