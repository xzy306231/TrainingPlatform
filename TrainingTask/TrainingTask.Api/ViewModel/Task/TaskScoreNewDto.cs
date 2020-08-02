using System;
using System.Collections.Generic;
using TrainingTask.Api.ViewModel.Subject;

namespace TrainingTask.Api.ViewModel.Task
{
    public class TaskScoreNewDto
    {
        /// <summary>
        /// 用户id
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        /// 计划id
        /// </summary>
        public long PlanId { get; set; }

        /// <summary>
        /// 任务id
        /// </summary>
        public long TaskId { get; set; }

        /// <summary>
        /// 任务结果
        /// </summary>
        public int Result { get; set; } = 1;

        /// <summary>
        /// 任务状态
        /// </summary>
        public int Status { get; set; } = 1;

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? StartTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? EndTime { get; set; }

        public IList<SubjectScoreNewDto> SubjectScores { get; set; } = new List<SubjectScoreNewDto>();
    }
}
