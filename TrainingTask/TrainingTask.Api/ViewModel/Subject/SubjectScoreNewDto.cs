namespace TrainingTask.Api.ViewModel.Subject
{
    public class SubjectScoreNewDto
    {
        /// <summary>
        /// 科目id
        /// </summary>
        public long SubjectId { get; set; }

        /// <summary>
        /// 科目结果
        /// </summary>
        public int Result { get; set; } = 1;

        /// <summary>
        /// 科目状态
        /// </summary>
        public int Status { get; set; } = 1;
    }
}
