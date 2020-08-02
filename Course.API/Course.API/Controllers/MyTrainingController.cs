using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Course.BLL;
using Course.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Course.API.Controllers
{
    /// <summary>
    /// 我的培训
    /// </summary>
    [Route("course/v1")]
    [ApiController]
    public class MyTrainingController : ControllerBase
    {
        private IHttpClientHelper client;
        public MyTrainingController(IHttpClientHelper client)
        {
            this.client = client;
        }

        /// <summary>
        /// 获取我的培训任务
        /// </summary>
        /// <param name="strStatus">状态</param>
        /// <param name="planName">计划名称</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="UserID">用户</param>
        /// <returns></returns>
        [HttpGet("GetMyTraining")]
        public JsonResult GetMyTraining(string strStatus, string planName, string startTime, string endTime, string UserID)
        {
            var requestJWT = Request.Headers["Authorization"];
            TokenModel obj = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenModel>(client.GetTokenJson(requestJWT).Result);
            return new JsonResult(new MyTraining().GetMyTraining(strStatus, planName, startTime, endTime, UserID, obj));
        }

        /// <summary>
        /// 获取培训内容记录
        /// </summary>
        /// <param name="PlanID">培训任务ID</param>
        /// <param name="UserID">学员账号</param>
        /// <returns></returns>
        [HttpGet("GetStuTrainingPlanContent")]
        public JsonResult GetStuTrainingPlanContent(long PlanID, string UserID)
        {
            return new JsonResult(new MyTraining().GetStuTrainingPlanContent(PlanID, UserID));
        }

        /// <summary>
        /// 获取课程信息
        /// </summary>
        /// <param name="CourseID"></param>
        /// <param name="RecordID"></param>
        /// <returns></returns>
        [HttpGet("GetCourseInfoByCourseID")]
        public JsonResult GetCourseInfoByCourseID(long CourseID, long RecordID)
        {
            return new JsonResult(new MyTraining().GetCourseInfoByID(CourseID, RecordID));
        }

        /// <summary>
        /// 获取学习结构图
        /// </summary>
        /// <param name="courseid"></param>
        /// <param name="recordID"></param>
        /// <returns></returns>
        [HttpGet("GetLearningCourseStruct")]
        public JsonResult GetLearningCourseStruct(long courseid, long recordID)
        {
            return new JsonResult(new MyTraining().GetLearningCourseStruct(courseid, recordID));
        }

        /// <summary>
        /// 检测学习条件
        /// </summary>
        /// <param name="ContentID">培训计划内容ID</param>
        /// <param name="UserID">学员ID</param>
        /// <returns></returns>
        [HttpGet("CheckLearningCondition")]
        public JsonResult CheckLearningCondition(long ContentID, string UserID)
        {
            return new JsonResult(new MyTraining().CheckLearningCondition(ContentID, UserID, client));
        }

        /// <summary>
        /// 创建学习记录数据
        /// </summary>
        /// <param name="ContentID">计划内容ID</param>
        /// <param name="UserID">学员ID</param>
        /// <returns></returns>
        [HttpPost("Add_LearningRecord")]
        public JsonResult Add_LearningRecord(long ContentID, string UserID)
        {
            var requestJWT = Request.Headers["Authorization"];
            TokenModel obj = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenModel>(client.GetTokenJson(requestJWT).Result);
            return new JsonResult(new MyTraining().Add_LearningRecord(ContentID, UserID, obj));
        }

        /// <summary>
        /// 查看培训计划下的所有学员
        /// </summary>
        /// <param name="PlanID"></param>
        /// <returns></returns>
        [HttpGet("GetStuFromPlan")]
        public JsonResult GetStuFromPlan(long PlanID)
        {
            string str = PubMethod.ReadConfigJsonData("DicUrl");
            //部门
            string strDepartment = client.GetRequest(str + "department_type").Result;
            DicModel dicDepartment = Newtonsoft.Json.JsonConvert.DeserializeObject<DicModel>(strDepartment);

            return new JsonResult(new MyTraining().GetStuFromPlan(PlanID, dicDepartment));
        }

        /// <summary>
        /// 课程学习
        /// </summary>
        /// <param name="RecordID"></param>
        /// <param name="NodeID"></param>
        /// <returns></returns>
        [HttpGet("LearningCoursewareByStructNodeID")]
        public JsonResult LearningCoursewareByStructNodeID(long RecordID, long NodeID)
        {
            return new JsonResult(new MyTraining().LearningCoursewareByStructNodeID(RecordID, NodeID));
        }

        /// <summary>
        /// 创建或修改课程节点学习记录
        /// </summary>
        /// <param name="RecordID">记录ID</param>
        /// <param name="SrcID">课程ID</param>
        /// <param name="StructID">节点ID</param>
        /// <param name="NodeStatus">节点状态</param>
        /// <param name="LearningTime">学习时长（单位：秒）</param>
        /// <returns></returns>
        [HttpPost("Add_CourseNodeLearningStatus")]
        public JsonResult Add_CourseNodeLearningStatus(long RecordID, long SrcID, long StructID, string NodeStatus, int LearningTime)
        {
            var requestJWT = Request.Headers["Authorization"];
            TokenModel obj = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenModel>(client.GetTokenJson(requestJWT).Result);
            return new JsonResult(new MyTraining().Add_CourseNodeLearningStatus(RecordID, SrcID, StructID, NodeStatus, LearningTime, obj, client));
            // return new JsonResult(new MyTraining().Add_CourseNodeLearningStatus(RecordID, SrcID, StructID, NodeStatus, LearningTime,client));
        }
    }
}