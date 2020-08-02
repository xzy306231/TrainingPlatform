using System;

namespace PeopleManager.Api.ViewModel
{
    /// <summary>
    /// 奖惩
    /// </summary>
    public class RewardsAndPunishmentDto
    {
        /// <summary>
        /// 事件名称
        /// </summary>
        public string EventName { get; set; }

        /// <summary>
        /// 事件类别key
        /// </summary>
        public string EventTypeKey { get; set; }

        /// <summary>
        /// 事件类别value
        /// </summary>
        public string EventTypeValue { get; set; }

        /// <summary>
        /// 发生日期
        /// </summary>
        public DateTime? EventDate { get; set; }

        /// <summary>
        /// 事件结果
        /// </summary>
        public string EventResult { get; set; }
    }
}
