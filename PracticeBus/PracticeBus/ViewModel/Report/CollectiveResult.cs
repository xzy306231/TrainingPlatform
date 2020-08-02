namespace PracticeBus.ViewModel.Report
{
    public class CollectiveResult
    {
        /// <summary>
        /// 参与人数
        /// </summary>
        public string AttendNumb { get; set; } = "0";

        /// <summary>
        /// 参与人数超出/低于
        /// </summary>
        public string AttendFlag { get; set; } = "0";

        /// <summary>
        /// 超出/低于平均的人数
        /// </summary>
        public string AttendNumbDif { get; set; } = "0";

        /// <summary>
        /// 模拟练习总时长
        /// </summary>
        public string TotalDuration { get; set; } = "0";

        /// <summary>
        /// 平均时长
        /// </summary>
        public string AvgDuration { get; set; } = "0";

        /// <summary>
        /// 所有培训平均练习时长
        /// </summary>
        public string TotalAvgDuration { get; set; } = "0";

        /// <summary>
        /// 超出/低于平均的时长标志
        /// </summary>
        public string AvgDurationFlag { get; set; } = "持平";

        /// <summary>
        /// 超出/低于平均的时长数
        /// </summary>
        public string AvgDurationDif { get; set; } = "0";

        /// <summary>
        /// 科目总数
        /// </summary>
        public string TotalSubjectNumb { get; set; } = "0";

        /// <summary>
        /// 完成科目数
        /// </summary>
        public string FinishSubjectNumb { get; set; } = "0";

        /// <summary>
        /// 平均完成率
        /// </summary>
        public string AvgFinishSubject { get; set; } = "0";

        /// <summary>
        /// 所有培训平均完成率
        /// </summary>
        public string TotalAvgFinishSubject { get; set; } = "0";

        /// <summary>
        /// 超出/低于平均完成率标志
        /// </summary>
        public string AvgFinishSubjectFlag { get; set; } = "持平";

        /// <summary>
        /// 超出/低于平均完成的百分比
        /// </summary>
        public string AvgFinishSubjectDif { get; set; } = "0";

        /// <summary>
        /// 通过科目数
        /// </summary>
        public string PassSubjectNumb { get; set; } = "0";

        /// <summary>
        /// 平均通过率
        /// </summary>
        public string AvgPassSubject { get; set; } = "0";

        /// <summary>
        /// 所有培训平均通过率
        /// </summary>
        public string TotalAvgPassSubject { get; set; } = "0";

        /// <summary>
        /// 超出/低于平均通过率标志
        /// </summary>
        public string AvgPassSubjectFlag { get; set; } = "持平";

        /// <summary>
        /// 超出/低于平均通过的百分比
        /// </summary>
        public string AvgPassSubjectDif { get; set; } = "0";

        /// <summary>
        /// 模拟训练整体结果，0：未达标，1：达标
        /// </summary>
        public string TaskGlobalResult { get; set; } = "未达标";
    }
}
