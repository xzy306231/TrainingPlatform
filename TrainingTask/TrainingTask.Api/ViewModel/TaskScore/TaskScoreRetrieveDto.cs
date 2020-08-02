namespace TrainingTask.Api.ViewModel.TaskScore
{
    public class TaskScoreRetrieveDto
    {
        /// <summary>
        /// id 
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 用户id
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 所属部门
        /// </summary>
        public string Department { get; set; }

        /// <summary>
        /// 训练结果
        /// </summary>
        public string Result { get; set; }

        /// <summary>
        /// 训练状态
        /// </summary>
        public string Status { get; set; }
    }
}
