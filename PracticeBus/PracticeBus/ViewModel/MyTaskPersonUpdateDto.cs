using System.Collections.Generic;

namespace PracticeBus.ViewModel
{
    public class MyTaskPersonUpdateDto
    {
        /// <summary>
        /// 计划id
        /// </summary>
        public long PlanId { get; set; }

        /// <summary>
        /// 任务id
        /// </summary>
        public List<long> TaskId { get; set; }

        /// <summary>
        /// 新增人员信息
        /// </summary>
        public List<UserInfo> NewUsers { get; set; }

        /// <summary>
        /// 删除人员信息
        /// </summary>
        public List<UserInfo> RemoveUsers { get; set; }
    }

    public class UserInfo
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
    }
}
