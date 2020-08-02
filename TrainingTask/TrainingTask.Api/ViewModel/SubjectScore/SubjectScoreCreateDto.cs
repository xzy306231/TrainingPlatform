namespace TrainingTask.Api.ViewModel.SubjectScore
{
    public class SubjectScoreCreateDto
    {
        /// <summary>
        /// 科目id
        /// </summary>
        public long SubjectId { get; set; }

        /// <summary>
        /// 科目名称
        /// </summary>
        public string SubjectName { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Desc { get; set; }

        /// <summary>
        /// 标签展示
        /// </summary>
        public string TagDisplay { get; set; }

        /// <summary>
        /// 适用机型value
        /// </summary>
        public string AirplaneValue { get; set; }

        /// <summary>
        /// 分类value
        /// </summary>
        public string ClassifyValue { get; set; }

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
