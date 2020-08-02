using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrainingTask.Api.ViewModel.Subject
{
    public class SubjectStatistics
    {
        /// <summary>
        /// 完成率
        /// </summary>
        public float FinishPercent { get; set; }

        /// <summary>
        /// 通过率
        /// </summary>
        public float PassPercent { get; set; }
    }
}
