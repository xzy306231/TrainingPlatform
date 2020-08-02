using System.ComponentModel.DataAnnotations;

namespace Courseware.Api.ViewModel.Manage
{
    /// <summary>
    /// 课件审核操作
    /// </summary>
    public class CheckOption
    {
        /// <summary>
        /// 资源id
        /// </summary>
        [Required]
        public long Id { get; set; }

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

        /// <summary>
        /// 审核备注
        /// </summary>
        public string CheckRemark { get; set; }
    }
}
