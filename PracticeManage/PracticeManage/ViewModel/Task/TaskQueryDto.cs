using System;

namespace PracticeManage.ViewModel.Task
{
    public class TaskQueryDto:BaseTaskDto
    {
        /// <summary>
        /// 主键
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 标签显示
        /// </summary>
        public string TagDisplay { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
    }
}
