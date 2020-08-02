using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Courseware.Api.ViewModel.Manage
{
    /// <summary>
    /// 知识点设置
    /// </summary>
    public class SetTagDto
    {
        [Required]
        public long Id { get; set; }

        public IList<KnowledgeTagDto> Tags { get; set; }
    }
}
