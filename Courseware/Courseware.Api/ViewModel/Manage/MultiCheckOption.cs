using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Courseware.Api.ViewModel.Manage
{
    /// <summary>
    /// 课件批量审核
    /// </summary>
    public class MultiCheckOption
    {
        /// <summary>
        /// 审核人id
        /// </summary>
        [Required]
        public long CheckerId { get; set; }

        /// <summary>
        /// 审核人名
        /// </summary>
        [Required]
        public string CheckerName { get; set; }

        [Required]
        public IEnumerable<long> CheckIdList { get; set; }
    }
}
