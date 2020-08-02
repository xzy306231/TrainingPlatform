using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PeopleManager.Core.Entity
{
    /// <summary>
    /// 奖惩
    /// </summary>
    [Table("t_rewards_and_punishment")]
    public class RewardsAndPunishmentEntity : BaseEntity
    {
        /// <summary>
        /// 平台中人员id
        /// </summary>
        [Display(Name = "人员id")]
        [Column("person_id")]
        public long PersonId { get; set; }

        [Display(Name = "事件名称")]
        [Column("event_name")]
        public string EventName { get; set; }

        [Display(Name = "事件类别key")]
        [Column("event_type_key")]
        public string EventTypeKey { get; set; }

        [Display(Name = "事件类别value")]
        [Column("event_type_value")]
        public string EventTypeValue { get; set; }

        [Display(Name = "发生日期")]
        [Column("event_date")]
        public DateTime? EventDate { get; set; }

        [Display(Name = "事件结果")]
        [Column("event_result")]
        public string EventResult { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public PersonInfoEntity PersonInfo { get; set; }
    }
}
