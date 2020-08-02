using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Courseware.Api.ViewModel.Manage.Resource
{
    /// <summary>
    /// 我的课件一览
    /// </summary>
    public class ResourceMineDto : ResourceDtoBase
    {
        /// <summary>
        /// 持续时长
        /// </summary>
        public int ResourceDuration { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public string CreatorName { get; set; }

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
