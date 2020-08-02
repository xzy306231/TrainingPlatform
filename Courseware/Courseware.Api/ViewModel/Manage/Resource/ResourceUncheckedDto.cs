using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Courseware.Api.ViewModel.Manage.Resource
{
    /// <summary>
    /// 课件未审核一览
    /// </summary>
    public class ResourceUnCheckedDto : ResourceDtoBase
    {
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

        public ICollection<ResourceTagDto> ResourceTags { get; set; }
    }
}
