using System;
using System.Collections.Generic;

namespace Courseware.Api.ViewModel.Manage.Resource
{
    /// <summary>
    /// 课件审核一览
    /// </summary>
    public class ResourceCheckedDto : ResourceDtoBase
    {
        /// <summary>
        /// 审核时间
        /// </summary>
        public DateTime CheckDate { get; set; }

        /// <summary>
        /// 审核人
        /// </summary>
        public string CheckerName { get; set; }

        /// <summary>
        /// 审核状态
        /// </summary>
        public string CheckStatus { get; set; }

        /// <summary>
        /// 审核备注
        /// </summary>
        public string CheckRemark { get; set; }

        public ICollection<ResourceTagDto> ResourceTags { get; set; }
    }
}
