using System.ComponentModel.DataAnnotations;

namespace Courseware.Api.ViewModel.FileUpDown
{
    public class FileMergeDto
    {
        [Required]
        public string name { get; set; }
    }
}
