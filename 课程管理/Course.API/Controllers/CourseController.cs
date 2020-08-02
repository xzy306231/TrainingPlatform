using Course.BLL;
using Course.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Course.API.Controllers
{
    /// <summary>
    /// 课程管理
    /// </summary>
    [Route("course/v1")]
    [ApiController]
    public class CourseController : ControllerBase
    {
        private readonly IHttpClientHelper client;
        private readonly CourseManagement course;
        private readonly pf_course_manage_v1Context db;
        private readonly IConfiguration Configuration;
        private readonly RabbitMQClient rabbit;
        public CourseController(IHttpClientHelper client, CourseManagement course, pf_course_manage_v1Context db, IConfiguration Configuration, RabbitMQClient rabbit)
        {
            this.client = client;
            this.course = course;
            this.db = db;
            this.Configuration = Configuration;
            this.rabbit = rabbit;
        }


        /// <summary>
        /// 获取课程信息
        /// </summary>
        /// <param name="queryCriteria"></param>
        /// <returns></returns>
        [HttpPut("GetCourseInfo")]
        public JsonResult GetCourseInfo(QueryCriteria queryCriteria)
        {
            return new JsonResult(course.GetCourse(db,queryCriteria));
        }

        /// <summary>
        /// 根据ID获取课程信息
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        [HttpGet("GetCourseInfoByID")]
        public JsonResult GetCourseInfoByID(long ID)
        {
            return new JsonResult(course.GetCourseInfoByID(db, Configuration,ID));
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
            return new JsonResult(course.Add_Course(db,rabbit,coursetag, obj));
        }

        /// <summary>
        /// 修改课程
        /// </summary>
        /// <param name="course">课程对象</param>
        /// <returns></returns>
        [HttpPut("UpdateCourse")]
        public JsonResult UpdateCourse(CourseTag coursetag)
        {
            var requestJWT = Request.Headers["Authorization"];
            TokenModel obj = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenModel>(client.GetTokenJson(requestJWT).Result);
            return new JsonResult(course.Update_Course(db,rabbit,coursetag, obj));
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
            return new JsonResult(course.Update_CoursePublish(db,rabbit,courseid, obj));
        }

        /// <summary>
        /// 课程批量发布
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        [HttpPut("PublishBatchCourse")]
        public object PublishBatchCourse(List<long> list)
        {
            var requestJWT = Request.Headers["Authorization"];
            TokenModel obj = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenModel>(client.GetTokenJson(requestJWT).Result);
            return new JsonResult(course.PublishBatchCourse(db,rabbit,list,obj));
        }

        [HttpPut("RevertCourseStatus")]
        public async Task<object> RevertCourseStatus(long id)
        {
            var requestJWT = Request.Headers["Authorization"];
            TokenModel obj = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenModel>(client.GetTokenJson(requestJWT).Result);
            return new JsonResult(await course.RevertCourseStatus(db,rabbit,id,obj));
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
            return new JsonResult(course.Delete_Course(db,rabbit,id, strUserNumber, obj));
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
            return new JsonResult(course.Delete_BatchCourse(db,rabbit,courseListID, obj));
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
            return new JsonResult(course.SetBatchCourseBatchTag(db,rabbit,batchCourseTag, obj));
        }

        [HttpPut("SetBatchCourseBatchConfidential")]
        public JsonResult SetBatchCourseBatchConfidential(BatchCourseConfidential batchCourseConfidential)
        {
            return new JsonResult(course.SetBatchCourseBatchConfidential(db,batchCourseConfidential));
        }

        /// <summary>
        /// 获取课程结构
        /// </summary>
        /// <param name="courseid"></param>
        /// <returns></returns>
        [HttpGet("GetCourseStruct")]
        public JsonResult GetCourseStruct(long courseid)
        {
            return new JsonResult(course.GetCourseStruct(db,courseid));
        }

        /// <summary>
        /// 根据课程结构ID查看相应的资源信息
        /// </summary>
        /// <param name="NodeID">结构ID</param>
        /// <returns></returns>
        [HttpGet("ViewCoursewareByStructNodeID")]
        public JsonResult ViewCoursewareByStructNodeID(long NodeID)
        {
            return new JsonResult(course.ViewCoursewareByStructNodeID(db,Configuration,NodeID));
        }

        [HttpGet("PreviewPageContent")]
        public JsonResult PreviewPageContent(long nodeId)
        {
            return new JsonResult(course.PreviewPageContent(db,nodeId));
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
            return new JsonResult(course.Add_CourseStructNode(db,rabbit,CourseStruct, obj));
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
            return new JsonResult(course.Update_CourseStructNode(db,rabbit,id, NodeName, UpdateBy, obj));
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
            return new JsonResult(course.Delete_CourseStructNode(db,rabbit,CourseStructID, obj));
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
            return new JsonResult(course.Add_CourseStructResource(db,rabbit,StructReaource, obj,client));
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
            return new JsonResult(course.Delete_StructResource(db,rabbit,StructID, ResourceID, obj));
        }
    }
}