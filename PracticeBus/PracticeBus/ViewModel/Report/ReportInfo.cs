namespace PracticeBus.ViewModel.Report
{
    public class PersonalInfo
    {
        /// <summary>
        /// 用户id
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        /// 训练时长
        /// </summary>
        public float Duration { get; set; }

        /// <summary>
        /// 完成百分比
        /// </summary>
        public float FinishPercent { get; set; }

        /// <summary>
        /// 通过百分比
        /// </summary>
        public float PassPercent { get; set; }
    }

    public class PersonalResult
    {
        /// <summary>
        /// 本次训练总科目
        /// </summary>
        public string TotalSubjectNumb { get; set; }

        /// <summary>
        /// 完成科目
        /// </summary>
        public string FinishSubjectNumb { get; set; }

        #region 练习时长

        /// <summary>
        /// 总练习时长
        /// </summary>
        public string TaskTrainSumTime { get; set; }
        /// <summary>
        /// 练习时长排名
        /// </summary>
        public string TaskTrainTimeRank { get; set; }
        /// <summary>
        /// 超出时长百分比
        /// </summary>
        public string TaskTrainTimeExceedPercent { get; set; }
        /// <summary>
        /// 平均练习时长
        /// </summary>
        public string TaskTrainAvgLearningTime { get; set; }
        /// <summary>
        /// 超出、低于标识
        /// </summary>
        public string TaskTrainLevelFlag { get; set; }
        /// <summary>
        /// 超出低于水平线小时数
        /// </summary>
        public string TaskTrainDifHours { get; set; }

        #endregion

        #region 完成率

        /// <summary>
        /// 总完成率
        /// </summary>
        public string TaskFinishRate { get; set; }
        /// <summary>
        /// 完成率排名
        /// </summary>
        public string TaskFinishRateRank { get; set; }
        /// <summary>
        /// 超出的完成率百分比
        /// </summary>
        public string TaskFinishRateExceedPercent { get; set; }
        /// <summary>
        /// 平均完成率
        /// </summary>
        public string TaskFinishAvgRate { get; set; }
        /// <summary>
        /// 超出、低于标识
        /// </summary>
        public string TaskFinishRateFlag { get; set; }
        /// <summary>
        /// 超出、低于的完成率
        /// </summary>
        public string TaskFinishDifRate { get; set; }
        #endregion

        #region 通过率

        /// <summary>
        /// 科目通过率
        /// </summary>
        public string TaskPassRate { get; set; }
        /// <summary>
        /// 通过率排名
        /// </summary>
        public string TaskPassRateRank { get; set; }
        /// <summary>
        /// 超出的通过率百分比
        /// </summary>
        public string TaskPassRatePercent { get; set; }
        /// <summary>
        /// 平均通过率
        /// </summary>
        public string TaskPassAvgRate { get; set; }
        /// <summary>
        /// 超出、低于标识
        /// </summary>
        public string TaskPassRateFlag { get; set; }
        /// <summary>
        /// 超出、低于的通过率
        /// </summary>
        public string TaskPassDifRate { get; set; }

        #endregion

        /// <summary>
        /// 模拟训练整体结果，0：未达标，1：达标
        /// </summary>
        public string TaskGlobalResult { get; set; }
    }
}
