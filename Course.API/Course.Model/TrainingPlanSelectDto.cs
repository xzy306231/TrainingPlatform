using System;
using System.Collections.Generic;
using System.Text;

namespace Course.Model
{
    public class TrainingPlanSelectDto
    {
        public long ID { get; set; }
        /// <summary>
        /// 工号
        /// </summary>
        public string userNumber { get; set; }

        /// <summary>
        /// 名字
        /// </summary>
        public string userName { get; set; }

        /// <summary>
        /// 学历
        /// </summary>
        public string educationKey { get; set; }

        /// <summary>
        /// 所属部门
        /// </summary>
        public string departmentKey { get; set; }

        /// <summary>
        /// 机型
        /// </summary>
        public string airplaneModelKey { get; set; }

        /// <summary>
        /// 技术等级
        /// </summary>
        public string skillLevelKey { get; set; }

        /// <summary>
        /// 飞行时长
        /// </summary>
        public double totalDuration { get; set; }

        /// <summary>
        /// 飞行状态
        /// </summary>
        public string flyStatusKey { get; set; }

        public string photoPath { get; set; }
    }
}
