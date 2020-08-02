using System;
using System.Collections.Generic;
using TrainingTask.Api.ViewModel.SubjectScore;

namespace TrainingTask.Api.ViewModel.TaskScore
{
    public class TaskScoreCreateDto
    {
        /// <summary>
        /// 用户id
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 所属部门
        /// </summary>
        public string Department { get; set; }

        /// <summary>
        /// 计划id
        /// </summary>
        public long PlanId { get; set; }

        /// <summary>
        /// 任务id
        /// </summary>
        public long TaskId { get; set; }

        /// <summary>
        /// 任务名称
        /// </summary>
        public string TaskName { get; set; }

        /// <summary>
        /// 任务结果
        /// 默认为1 未通过
        /// </summary>
        public int Result { get; set; } = 1;

        /// <summary>
        /// 任务状态
        /// 默认为1 未完成
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

        /// <summary>
        /// 科目成绩
        /// </summary>
        public IList<SubjectScoreCreateDto> SubjectScores { get; set; } = new List<SubjectScoreCreateDto>();
    }
}
