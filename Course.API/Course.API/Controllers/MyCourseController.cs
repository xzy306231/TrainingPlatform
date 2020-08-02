using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Course.BLL;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Course.API.Controllers
{
    /// <summary>
    /// 我的课程
    /// </summary>
    [Route("course/v1")]
    [ApiController]
    public class MyCourseController : ControllerBase
    {
        /// <summary>
        /// 获取我的培训计划下拉列表
        /// </summary>
        /// <param name="UserID">用户ID</param>
        /// <returns></returns>
        [HttpGet("GetMyTrainingPlanLList")]
        public JsonResult GetMyTrainingPlanLList(string UserID)
        {
            return new JsonResult(new MyCourse().GetMyTrainingPlanLList(UserID));
        }
        /// <summary>
        /// 获取我的课程
        /// </summary>
        /// <param name="PlanID">计划ID</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="strKeyWord">关键字</param>
        /// <param name="UserID">用户ID</param>
        /// <param name="pageSize">记录数</param>
        /// <param name="pageIndex">页码</param>
        /// <returns></returns>
        [HttpGet("GetMyCourse")]
        public JsonResult GetMyCourse(long PlanID, string startTime, string endTime, string strKeyWord, string UserID, int pageSize, int pageIndex = 1)
        {
            return new JsonResult(new MyCourse().GetMyCourse(PlanID, startTime, endTime, strKeyWord, UserID, pageSize, pageIndex));
        }

        /// <summary>
        /// 获取课程的学习记录
        /// </summary>
        /// <param name="UserID">用户ID</param>
        /// <param name="PlanID">计划号</param>
        /// <param name="CourseID">课程ID</param>
        /// <returns></returns>
        [HttpGet("GetStuCourseLearningRecord")]
        public JsonResult GetStuCourseLearningRecord(string UserID, long PlanID, long CourseID)
        {
            return new JsonResult(new MyCourse().GetStuCourseLearningRecord(UserID, PlanID, CourseID));
        }

        /// <summary>
        /// 继续学习
        /// </summary>
        /// <param name="RecordID"></param>
        /// <returns>结构ID</returns>
        //[HttpGet("LearningContinue")]
        //public JsonResult LearningContinue(long RecordID)
        //{
        //    return new JsonResult(new MyCourse().LearningContinue(RecordID));
        //}
    }
}