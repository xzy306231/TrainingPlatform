namespace PracticeManage.ViewModel.Subject
{
    public class SubjectBusNewDto : BaseSubjectDto
    {
        /// <summary>
        /// 原始表id
        /// </summary>
        public long OriginalId { get; set; }

        /// <summary>
        /// 任务id
        /// todo
        /// </summary>
        public long TaskId { get; set; }

        /// <summary>
        /// 版本
        /// </summary>
        public int Version { get; set; }

        /// <summary>
        /// 标签展示
        /// </summary>
        public string TagDisplay { get; set; }

        /// <summary>
        /// 创建人id
        /// </summary>
        public virtual long CreatorId { get; set; }

        /// <summary>
        /// 创建人名
        /// </summary>
        public virtual string CreatorName { get; set; }

        //public List<TagDto> Tags { get; set; }
    }
}
