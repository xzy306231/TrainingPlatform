using System.Collections.Generic;
using TrainingTask.Api.ViewModel.Subject;

namespace TrainingTask.Api.ViewModel.Task
{
    /// <summary>
    /// 
    /// </summary>
    public class TaskUpdateDto
    {
        /// <summary>
        /// 任务id
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 任务名称
        /// </summary>
        public string TaskName { get; set; }

        /// <summary>
        /// 任务描述
        /// </summary>
        public string TaskDesc { get; set; }

        /// <summary>
        /// 任务类别key
        /// </summary>
        public string TaskTypeKey { get; set; }

        /// <summary>
        /// 类别等级key
        /// </summary>
        public string TypeLevelKey { get; set; }

        /// <summary>
        /// 级别等级key
        /// </summary>
        public string LevelKey { get; set; }

        /// <summary>
        /// 适用机型key
        /// </summary>
        public string AirplaneTypeKey { get; set; }

        /// <summary>
        /// 课时
        /// </summary>
        public int ClassHour { get; set; }

        /// <summary>
        /// 任务类别value
        /// </summary>
        public string TaskTypeValue { get; set; }

        /// <summary>
        /// 类别等级value
        /// </summary>
        public string TypeLevelValue { get; set; }

        /// <summary>
        /// 级别等级value
        /// </summary>
        public string LevelValue { get; set; }

        /// <summary>
        /// 适用机型value
        /// </summary>
        public string AirplaneTypeValue { get; set; }


        /// <summary>
        /// 新增内容
        /// </summary>
        public IList<SubjectNewDto> NewSubjects { get; set; }

        /// <summary>
        /// 删除内容
        /// </summary>
        public IList<long> RemoveSubjects { get; set; }
    }
}
