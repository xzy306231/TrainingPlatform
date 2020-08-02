using System;

namespace PeopleManager.Api.ViewModel
{
    public class LicenseInfoDto
    {
        /// <summary>
        /// 证照名称
        /// </summary>
        public string LicenseName { get; set; }

        /// <summary>
        /// 是否有效
        /// </summary>
        public string ValidKey { get; set; }

        /// <summary>
        /// 是否有效
        /// </summary>
        public string ValidValue { get; set; }

        /// <summary>
        /// 开始日期
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// 结束日期
        /// </summary>
        public DateTime? EndDate { get; set; }
    }
}
