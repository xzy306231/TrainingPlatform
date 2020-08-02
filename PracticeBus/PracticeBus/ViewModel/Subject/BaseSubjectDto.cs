namespace PracticeBus.ViewModel.Subject
{
    public class BaseSubjectDto
    {
        /// <summary>
        /// 编号
        /// </summary>
        public string Number { get; set; }

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
        public string PlaneTypeKey { get; set; }

        /// <summary>
        /// 适用机型value
        /// </summary>
        public string PlaneTypeValue { get; set; }

        /// <summary>
        /// 预期结果
        /// </summary>
        public string ExpectResult { get; set; }

        /// <summary>
        /// 创建人id
        /// </summary>
        public long CreatorId { get; set; }

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
