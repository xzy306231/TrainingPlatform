using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace Course.API.Controllers
{
    /// <summary>
    /// 远程服务
    /// </summary>
    [Route("course/v1")]
    [ApiController]
    public class RemoteServiceController : ControllerBase
    {
        private readonly RemoteService remote;
        private readonly pf_course_manage_v1Context db;
        public RemoteServiceController(RemoteService remote, pf_course_manage_v1Context db)
        {
            this.remote = remote;
            this.db = db;
        }
        [HttpPost("GetCourseInfomationByID")]
        public JsonResult GetCourseInfomationByID(List<PlanID> idList)
        {
            return new JsonResult(remote.GetCourseInfomationByID(db,idList));
        }

        [HttpGet("TrainingPlanFromProgram")]
        public JsonResult TrainingPlanFromProgram(long programId)
        {
            return new JsonResult(remote.TrainingPlanFromProgram(db,programId));
        }

        [HttpGet("GetCourseIDByProgramID")]
        public object GetCourseIDByProgramID(long id)
        {
            return new JsonResult(remote.GetCourseIDByProgramID(db,id));
        }

        [HttpGet("GetTaskIDByProgramID")]
        public object GetTaskIDByProgramID(long id)
        {
            return new JsonResult(remote.GetTaskIDByProgramID(db,id));
        }
    }
}