using System.ComponentModel.DataAnnotations;

namespace Courseware.Api.ViewModel.Manage
{
    /// <summary>
    /// 课件重命名
    /// </summary>
    public class RenameDto
    {
        [Required]
        public long Id { get; set; }

        [Required]
        public string ResourceName { get; set; }
    }
}
