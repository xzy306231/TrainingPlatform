using CoursewareDev.BLL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoursewareDev.API.Controllers
{
    /// <summary>
    /// 课件开发管理
    /// </summary>
    [Route("coursewaredev/v1")]
    [ApiController]
    public class CoursewareController : ControllerBase
    {
        private readonly pf_courseware_devContext db;
        private readonly IHttpClientHelper client;
        private readonly ILogger<CoursewareController> log;
        private readonly Courseware courseware;
        private readonly RabbitMQClient rabbit;
        public CoursewareController(pf_courseware_devContext db,
                                    ILogger<CoursewareController> log,
                                    Courseware courseware, 
                                    RabbitMQClient rabbit, 
                                    IHttpClientHelper client)
        {
            this.db = db;
            this.log = log;
            this.courseware = courseware;
            this.rabbit = rabbit;
            this.client = client;
        }
        /// <summary>
        /// 获取课件开发信息(未发布)
        /// </summary>
        /// <param name="keyWord"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet("GetNotPublishCourseware")]
        public JsonResult GetNotPublishCourseware(string keyWord, int pageIndex = 1, int pageSize = 10)
        {
            return new JsonResult(courseware.GetNotPublishCourseware(db, keyWord, pageIndex, pageSize));
        }

        /// <summary>
        /// 获取课件开发信息(已发布)
        /// </summary>
        /// <param name="keyWord"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet("GetPublishedCourseware")]
        public JsonResult GetPublishedCourseware(string keyWord, int pageIndex = 1, int pageSize = 10)
        {
            return new JsonResult(courseware.GetPublishedCourseware(db, keyWord, pageIndex, pageSize));
        }

        /// <summary>
        /// 发布课件
        /// </summary>
        /// <param name="idList"></param>
        /// <returns></returns>
        [HttpPut("PublishCourseware")]
        public async Task<object> PublishCourseware(List<long> idList)
        {
            return new JsonResult(await courseware.PublishCourseware(db, idList));
        }
        /// <summary>
        /// 删除课件
        /// </summary>
        /// <param name="idList"></param>
        /// <returns></returns>
        [HttpPut("RemoveCourseware")]
        public async Task<object> RemoveCourseware(List<long> idList)
        {
            return new JsonResult(await courseware.RemoveCourseware(db, idList));
        }
        /// <summary>
        /// 创建课件
        /// </summary>
        /// <param name="ware"></param>
        /// <returns></returns>
        [HttpPost("CreateCourseware")]
        public async Task<object> CreateCourseware(CoursewareInfo ware)
        {
            return new JsonResult(await courseware.CreateCourseware(db, ware));
        }
        /// <summary>
        /// 查看课件内容
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("GetPageContent")]
        public object GetPageContent(long id)
        {
            return new JsonResult(courseware.GetPageContent(db, id));
        }
        /// <summary>
        /// 课件预览
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("PreviewPageContent")]
        public object PreviewPageContent(long id)
        {
            return new JsonResult(courseware.PreviewPageContent(db, id));
        }
        /// <summary>
        /// 保存课件
        /// </summary>
        /// <param name="coursewareInfo"></param>
        /// <returns></returns>
        [HttpPost("SaveCourseware")]
        public async Task<object> SaveCourseware(CoursewareInfo coursewareInfo)
        {
            return new JsonResult(await courseware.SaveCourseware(db, coursewareInfo));
        }
        [HttpPost("Test")]
        public JsonResult Test(List<CoursewareInfo> coursewareInfos)
        {
            return null;
        }
    }
}