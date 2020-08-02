using System;
using System.Collections.Generic;

namespace Courseware.Api.ViewModel.Manage.Resource
{
    /// <summary>
    /// 课件一览
    /// </summary>
    public class ResourceAllDto : ResourceDtoBase
    {
        /// <summary>
        /// 资源描述
        /// </summary>
        public string ResourceDesc { get; set; }

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

        public ICollection<ResourceTagDto> ResourceTags { get; set; }
    }
}
