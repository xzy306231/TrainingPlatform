namespace PracticeManage.ViewModel.Subject
{
    public class BaseSubjectDto
    {
        /// <summary>
        /// 编号
        /// </summary>
        public virtual string Number { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public virtual string Desc { get; set; }

        /// <summary>
        /// 分类key
        /// </summary>
        public virtual string ClassifyKey { get; set; }

        /// <summary>
        /// 分类value
        /// </summary>
        public virtual string ClassifyValue { get; set; }

        /// <summary>
        /// 适用机型key
        /// </summary>
        public virtual string PlaneTypeKey { get; set; }

        /// <summary>
        /// 适用机型value
        /// </summary>
        public virtual string PlaneTypeValue { get; set; }

        /// <summary>
        /// 预期结果
        /// </summary>
        public virtual string ExpectResult { get; set; }
    }
}
