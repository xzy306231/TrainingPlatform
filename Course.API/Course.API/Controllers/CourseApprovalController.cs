using Course.BLL;
using Course.Model;
using Microsoft.AspNetCore.Mvc;
using System.IO;

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
        public CourseApprovalController(IHttpClientHelper client)
        {
            this.client = client;
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
            return new JsonResult(new CourseApproval().Update_CourseApprovalByID(approval,obj));
        }

        /// <summary>
        /// 已审核
        /// </summary>
        /// <param name="queryCriteria"></param>
        /// <returns></returns>
        [HttpPut("GetCourseAboutApprovaled")]
        public JsonResult GetCourseAboutApprovaled(Query queryCriteria)
        {
            return new JsonResult(new CourseApproval().GetCourseAboutApprovaled(queryCriteria));
        }

        /// <summary>
        /// 未审核
        /// </summary>
        /// <param name="queryCriteria"></param>
        /// <returns></returns>
        [HttpPut("GetCourseAboutNotApprovaled")]
        public JsonResult GetCourseAboutNotApprovaled(Query queryCriteria)
        {
            return new JsonResult(new CourseApproval().GetCourseAboutNotApprovaled(queryCriteria));
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
            return new JsonResult(new CourseApproval().Update_CourseApprovalBatch(approvalResult,obj));
        }

        [HttpGet("GetPath")]
        public JsonResult GetPath()
        {
            string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + "/ErrorLog/";
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            return new JsonResult(path);
        }
    }
}