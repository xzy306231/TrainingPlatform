namespace PeopleManager.Api.ViewModel
{
    public class PersonInfoDto
    {
        /// <summary>
        /// id
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 原始表id
        /// </summary>
        public long OriginalId { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 工号
        /// </summary>
        public string UserNumber { get; set; }

        /// <summary>
        /// 人员密级
        /// </summary>
        public int SecLevel { get; set; }

        public WorkOfPersonDto WorkInfo { get; set; }
    }
}
