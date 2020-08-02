using Course.BLL;
using Course.Model;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Course.API.Controllers
{
    /// <summary>
    /// 课程管理
    /// </summary>
    [Route("course/v1")]
    [ApiController]
    public class CourseController : ControllerBase
    {
        private IHttpClientHelper client;
        public CourseController(IHttpClientHelper client)
        {
            this.client = client;
        }


        /// <summary>
        /// 获取课程信息
        /// </summary>
        /// <param name="queryCriteria"></param>
        /// <returns></returns>
        [HttpPut("GetCourseInfo")]
        public JsonResult GetCourseInfo(QueryCriteria queryCriteria)
        {

            return new JsonResult(new CourseManagement().GetCourse(queryCriteria));
        }

        /// <summary>
        /// 根据ID获取课程信息
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        [HttpGet("GetCourseInfoByID")]
        public JsonResult GetCourseInfoByID(long ID)
        {
            return new JsonResult(new CourseManagement().GetCourseInfoByID(ID));
        }

        /// <summary>
        /// 创建课程
        /// </summary>
        /// <param name="coursetag"></param>
        /// <returns></returns>
        [HttpPost("CreateCourse")]
        public JsonResult CreateCourse(CourseTag coursetag)
        {
            var requestJWT = Request.Headers["Authorization"];
            TokenModel obj = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenModel>(client.GetTokenJson(requestJWT).Result);
            return new JsonResult(new CourseManagement().Add_Course(coursetag, obj));
        }

        /// <summary>
        /// 修改课程
        /// </summary>
        /// <param name="course">课程对象</param>
        /// <returns></returns>
        [HttpPut("UpdateCourse")]
        public JsonResult UpdateCourse(CourseTag course)
        {
            var requestJWT = Request.Headers["Authorization"];
            TokenModel obj = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenModel>(client.GetTokenJson(requestJWT).Result);
            return new JsonResult(new CourseManagement().Update_Course(course, obj));
        }

        /// <summary>
        /// 课程发布
        /// </summary>
        /// <param name="courseid">课程ID</param>
        /// <returns></returns>
        [HttpPut("CoursePublish")]
        public JsonResult CoursePublish(int courseid)
        {
            var requestJWT = Request.Headers["Authorization"];
            TokenModel obj = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenModel>(client.GetTokenJson(requestJWT).Result);
            return new JsonResult(new CourseManagement().Update_CoursePublish(courseid, obj));
        }

        /// <summary>
        /// 删除课程
        /// </summary>
        /// <param name="id"></param>
        /// <param name="strUserNumber"></param>
        /// <returns></returns>
        [HttpDelete("RemoveCourse")]
        public JsonResult RemoveCourse(long id, string strUserNumber)
        {
            var requestJWT = Request.Headers["Authorization"];
            TokenModel obj = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenModel>(client.GetTokenJson(requestJWT).Result);
            return new JsonResult(new CourseManagement().Delete_Course(id, strUserNumber, obj));
        }

        /// <summary>
        /// 批量删除课程
        /// </summary>
        /// <param name="courseListID"></param>
        /// <returns></returns>
        [HttpDelete("Delete_BatchCourse")]
        public JsonResult Delete_BatchCourse(CourseList courseListID)
        {
            var requestJWT = Request.Headers["Authorization"];
            TokenModel obj = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenModel>(client.GetTokenJson(requestJWT).Result);
            return new JsonResult(new CourseManagement().Delete_BatchCourse(courseListID, obj));
        }

        /// <summary>
        /// 批量设置课程知识点
        /// </summary>
        /// <param name="batchCourseTag"></param>
        /// <returns></returns>
        [HttpPost("SetBatchCourseBatchTag")]
        public JsonResult SetBatchCourseBatchTag(BatchCourseTag batchCourseTag)
        {
            var requestJWT = Request.Headers["Authorization"];
            TokenModel obj = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenModel>(client.GetTokenJson(requestJWT).Result);
            return new JsonResult(new CourseManagement().SetBatchCourseBatchTag(batchCourseTag, obj));
        }

        /// <summary>
        /// 获取课程结构
        /// </summary>
        /// <param name="courseid"></param>
        /// <returns></returns>
        [HttpGet("GetCourseStruct")]
        public JsonResult GetCourseStruct(long courseid)
        {
            return new JsonResult(new CourseManagement().GetCourseStruct(courseid));
        }

        /// <summary>
        /// 根据课程结构ID查看相应的资源信息
        /// </summary>
        /// <param name="NodeID">结构ID</param>
        /// <returns></returns>
        [HttpGet("ViewCoursewareByStructNodeID")]
        public JsonResult ViewCoursewareByStructNodeID(long NodeID)
        {
            return new JsonResult(new CourseManagement().ViewCoursewareByStructNodeID(NodeID));
        }

        /// <summary>
        /// 创建课程结构节点
        /// </summary>
        /// <param name="CourseStruct">课程结构对象</param>
        /// <returns></returns>
        [HttpPost("CreateCourseStructNode")]
        public JsonResult CreateCourseStructNode(CourseStructNode CourseStruct)
        {
            var requestJWT = Request.Headers["Authorization"];
            TokenModel obj = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenModel>(client.GetTokenJson(requestJWT).Result);
            return new JsonResult(new CourseManagement().Add_CourseStructNode(CourseStruct, obj));
        }

        /// <summary>
        /// 修改课程结构节点
        /// </summary>
        /// <param name="id">ID</param>
        /// <param name="NodeName">节点名称</param>
        /// <param name="UpdateBy">修改人ID</param>
        /// <returns></returns>
        [HttpPut("UpdateCourseStructNode")]
        public JsonResult UpdateCourseStructNode(int id, string NodeName, long UpdateBy)
        {
            var requestJWT = Request.Headers["Authorization"];
            TokenModel obj = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenModel>(client.GetTokenJson(requestJWT).Result);
            return new JsonResult(new CourseManagement().Update_CourseStructNode(id, NodeName, UpdateBy, obj));
        }

        /// <summary>
        /// 删除课程结构
        /// </summary>
        /// <param name="CourseStructID">结构ID</param>
        /// <returns></returns>
        [HttpDelete("RemoveCourseStructNode")]
        public JsonResult RemoveCourseStructNode(int CourseStructID)
        {
            var requestJWT = Request.Headers["Authorization"];
            TokenModel obj = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenModel>(client.GetTokenJson(requestJWT).Result);
            return new JsonResult(new CourseManagement().Delete_CourseStructNode(CourseStructID, obj));
        }

        /// <summary>
        /// 创建结构资源
        /// </summary>
        /// <param name="StructReaource">结构资源</param>
        /// <returns></returns>
        [HttpPost("CreateCourseStructResource")]
        public JsonResult CreateCourseStructResource(StructResource StructReaource)
        {
            var requestJWT = Request.Headers["Authorization"];
            TokenModel obj = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenModel>(client.GetTokenJson(requestJWT).Result);
            return new JsonResult(new CourseManagement().Add_CourseStructResource(StructReaource, obj));
        }

        /// <summary>
        /// 删除结构资源
        /// </summary>
        /// <param name="StructID">结构ID</param>
        /// <param name="ResourceID">资源ID</param>
        /// <returns></returns>
        [HttpDelete("Delete_StructResource")]
        public JsonResult Delete_StructResource(long StructID, long ResourceID)
        {
            var requestJWT = Request.Headers["Authorization"];
            TokenModel obj = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenModel>(client.GetTokenJson(requestJWT).Result);
            return new JsonResult(new CourseManagement().Delete_StructResource(StructID, ResourceID, obj));
        }
    }
}