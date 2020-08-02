using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PracticeBus.ViewModel.Task
{
    public class AddMyTaskParams
    {
        /// <summary>
        /// 培训计划id
        /// </summary>
        public long PlanId { get; set; }

        /// <summary>
        /// 创建人id
        /// </summary>
        public long CreatorId { get; set; }

        /// <summary>
        /// 教员用户名
        /// </summary>
        public string TeacherNum { get; set; }

        /// <summary>
        /// 教员名称
        /// </summary>
        public string TeacherName { get; set; }

        [Required]
        public List<TaskNewDto> Tasks { get; set; } = new List<TaskNewDto>();
    }
}
