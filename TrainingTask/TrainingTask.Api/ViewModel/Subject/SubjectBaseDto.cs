namespace TrainingTask.Api.ViewModel.Subject
{
    public abstract class SubjectBaseDto
    {
        /// <summary>
        /// 原始表id
        /// </summary>
        public long OriginalId { get; set; }

        /// <summary>
        /// 编号
        /// </summary>
        public string SubjectNumb { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Desc { get; set; }

        /// <summary>
        /// 分类key
        /// </summary>
        public string ClassifyKey { get; set; }

        /// <summary>
        /// 分类value
        /// </summary>
        public string ClassifyValue { get; set; }

        /// <summary>
        /// 适用机型key
        /// </summary>
        public string AirplaneKey { get; set; }

        /// <summary>
        /// 适用机型value
        /// </summary>
        public string AirplaneValue { get; set; }

        /// <summary>
        /// 预期结果
        /// </summary>
        public string ExpectedResult { get; set; }

        /// <summary>
        /// 创建人名
        /// </summary>
        public string CreatorName { get; set; }

        /// <summary>
        /// 标签展示
        /// </summary>
        public string TagDisplay { get; set; }
    }
}
