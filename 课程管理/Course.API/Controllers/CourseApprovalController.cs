using Course.BLL;
using Course.Model;
using Microsoft.AspNetCore.Mvc;

namespace Course.API.Controllers
{
    /// <summary>
    /// 课程审核
    /// </summary>
    [Route("course/v1")]
    [ApiController]
    public class CourseApprovalController : ControllerBase
    {
        private IHttpClientHelper client;
        private CourseApproval courseApproval;
        private readonly pf_course_manage_v1Context db;
        private readonly RabbitMQClient rabbit;
        public CourseApprovalController(IHttpClientHelper client, CourseApproval courseApproval, pf_course_manage_v1Context db,RabbitMQClient rabbit)
        {
            this.client = client;
            this.courseApproval = courseApproval;
            this.db = db;
            this.rabbit = rabbit;
        }
        /// <summary>
        /// 课程审核
        /// </summary>
        /// <param name="approval"></param>
        /// <returns></returns>
        [HttpPut("CourseApprovalByID")]
        public JsonResult CourseApprovalByID(Approval approval)
        {
            var requestJWT = Request.Headers["Authorization"];
            TokenModel obj = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenModel>(client.GetTokenJson(requestJWT).Result);
            return new JsonResult(courseApproval.Update_CourseApprovalByID(db,rabbit,approval, obj));
        }

        /// <summary>
        /// 已审核
        /// </summary>
        /// <param name="queryCriteria"></param>
        /// <returns></returns>
        [HttpPut("GetCourseAboutApprovaled")]
        public JsonResult GetCourseAboutApprovaled(Query queryCriteria)
        {
            return new JsonResult(courseApproval.GetCourseAboutApprovaled(db,queryCriteria));
        }

        /// <summary>
        /// 未审核
        /// </summary>
        /// <param name="queryCriteria"></param>
        /// <returns></returns>
        [HttpPut("GetCourseAboutNotApprovaled")]
        public JsonResult GetCourseAboutNotApprovaled(Query queryCriteria)
        {
            return new JsonResult(courseApproval.GetCourseAboutNotApprovaled(db,queryCriteria));
        }

        /// <summary>
        /// 批量审核
        /// </summary>
        /// <param name="approvalResult">审核结果</param>
        /// <returns></returns>
        [HttpPut("Update_CourseApprovalBatch")]
        public JsonResult Update_CourseApprovalBatch(ApprovalResult approvalResult)
        {
            var requestJWT = Request.Headers["Authorization"];
            TokenModel obj = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenModel>(client.GetTokenJson(requestJWT).Result);
            return new JsonResult(courseApproval.Update_CourseApprovalBatch(db,rabbit,approvalResult, obj));
        }

        /// <summary>
        /// 获取审核通过的课程
        /// </summary>
        /// <param name="queryCriteria"></param>
        /// <returns></returns>
        [HttpPut("GetApprovalCourse")]
        public JsonResult GetApprovalCourse(QueryCriteria queryCriteria)
        {
            return new JsonResult(courseApproval.GetApprovalCourse(db,queryCriteria));
        }

    }
}