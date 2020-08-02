using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

[Route("trainingplan/v1")]
[ApiController]
public class LessonScheduleController : ControllerBase
{
    private readonly IHttpClientHelper client;
    private readonly LessonSchedule lessonSchedule;
    private readonly IConfiguration configuration;
    private readonly RabbitMQClient rabbit;
    private readonly pf_training_plan_v1Context db;
    public LessonScheduleController(IHttpClientHelper client, LessonSchedule lessonSchedule, IConfiguration configuration, RabbitMQClient rabbit, pf_training_plan_v1Context db)
    {
        this.db = db;
        this.client = client;
        this.lessonSchedule = lessonSchedule;
        this.configuration = configuration;
        this.rabbit = rabbit;

    }

    #region 教室管理
    /// <summary>
    /// 查询教室列表
    /// </summary>
    /// <param name="classroomType">教室类型</param>
    /// <param name="classroomName">教室名称</param>
    /// <param name="classroomStatus">教室状态</param>
    /// <param name="pageIndex">页码</param>
    /// <param name="pageSize">记录数</param>
    /// <returns></returns>
    [HttpGet("GetClassroom")]
    public object GetClassroom(string classroomType, string classroomName, string classroomStatus, int pageIndex = 1, int pageSize = 10)
    {
        return new JsonResult(lessonSchedule.GetClassroom(db, classroomType, classroomName, classroomStatus, pageIndex, pageSize));
    }

    /// <summary>
    /// 创建教室
    /// </summary>
    /// <param name="classroom"></param>
    /// <returns></returns>
    [HttpPost("AddClassroom")]
    public async Task<object> AddClassroom([FromBody]Classroom classroom)
    {
        var requestJWT = Request.Headers["Authorization"];
        TokenModel obj = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenModel>(client.GetTokenJson(requestJWT).Result);
        return new JsonResult(await lessonSchedule.AddClassroom(db, classroom, obj, rabbit));
    }
    /// <summary>
    /// 修改教室
    /// </summary>
    /// <param name="classroom"></param>
    /// <returns></returns>
    [HttpPut("EditClassroom")]
    public async Task<object> EditClassroom(Classroom classroom)
    {
        var requestJWT = Request.Headers["Authorization"];
        TokenModel obj = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenModel>(client.GetTokenJson(requestJWT).Result);
        return new JsonResult(await lessonSchedule.EditClassroom(db, classroom, obj, rabbit));
    }
    /// <summary>
    /// 启用教室
    /// </summary>
    /// <param name="list"></param>
    /// <returns></returns>
    [HttpPut("StartUse")]
    public async Task<object> StartUse(List<long> list)
    {
        var requestJWT = Request.Headers["Authorization"];
        TokenModel obj = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenModel>(client.GetTokenJson(requestJWT).Result);
        return new JsonResult(await lessonSchedule.StartUse(db, list, obj, rabbit));
    }
    /// <summary>
    /// 停用教室
    /// </summary>
    /// <param name="list"></param>
    /// <returns></returns>
    [HttpPut("StopUse")]
    public async Task<object> StopUse(List<long> list)
    {
        var requestJWT = Request.Headers["Authorization"];
        TokenModel obj = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenModel>(client.GetTokenJson(requestJWT).Result);
        return new JsonResult(await lessonSchedule.StopUse(db, list, obj, rabbit));
    }
    /// <summary>
    /// 删除教室
    /// </summary>
    /// <param name="list"></param>
    /// <returns></returns>
    [HttpPut("RemoveClassrooms")]
    public async Task<object> RemoveClassrooms(List<long> list)
    {
        var requestJWT = Request.Headers["Authorization"];
        TokenModel obj = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenModel>(client.GetTokenJson(requestJWT).Result);
        return new JsonResult(await lessonSchedule.RemoveClassrooms(db, list, obj, rabbit));
    }
    #endregion

    #region 排课课表

    /// <summary>
    /// 获取所有的培训计划
    /// </summary>
    /// <returns></returns>
    [HttpGet("GetAllTrainingPlan")]
    public object GetAllTrainingPlan()
    {
        return new JsonResult(lessonSchedule.GetAllTrainingPlan(db));
    }

    /// <summary>
    /// 获取所有未开始、进行中的培训计划
    /// </summary>
    /// <returns></returns>
    [HttpGet("GetNotEndTrainingPlan")]
    public object GetNotEndTrainingPlan()
    {
        return new JsonResult(lessonSchedule.GetNotEndTrainingPlan(db));
    }
    /// <summary>
    /// 获取所有启用的教室
    /// </summary>
    /// <returns></returns>
    [HttpGet("GetAllClassroom")]
    public object GetAllClassroom(string roomType)
    {
        return new JsonResult(lessonSchedule.GetAllClassroom(db,roomType));
    }

    /// <summary>
    /// 查询课程表信息
    /// </summary>
    /// <param name="planID"></param>
    /// <param name="trainingType"></param>
    /// <param name="startDate"></param>
    /// <param name="classroomId"></param>
    /// <returns></returns>
    [HttpGet("GetScheduleInfo")]
    public object GetScheduleInfo( long planID, string trainingType, string startDate, long classroomId)
    {
        return new JsonResult(lessonSchedule.GetScheduleInfo(db,planID,trainingType,startDate,classroomId));
    }
    /// <summary>
    /// 添加课程表内容
    /// </summary>
    /// <param name="trainingSchedule"></param>
    /// <returns></returns>
    [HttpPost("AddTrainingSchedule")]
    public async Task<object> AddTrainingSchedule(TrainingSchedule trainingSchedule)
    {
        var requestJWT = Request.Headers["Authorization"];
        TokenModel obj = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenModel>(client.GetTokenJson(requestJWT).Result);
        return new JsonResult(await lessonSchedule.AddTrainingSchedule(db, trainingSchedule, obj));
    }
    /// <summary>
    /// 修改课程表内容
    /// </summary>
    /// <param name="trainingSchedule"></param>
    /// <returns></returns>
    [HttpPut("EditTrainingSchedule")]
    public async Task<object> EditTrainingSchedule( TrainingSchedule trainingSchedule)
    {
        var requestJWT = Request.Headers["Authorization"];
        TokenModel obj = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenModel>(client.GetTokenJson(requestJWT).Result);
        return new JsonResult(await lessonSchedule.EditTrainingSchedule(db, trainingSchedule, obj));
    }

    /// <summary>
    /// 清除课程表内容
    /// </summary>
    /// <param name="list"></param>
    /// <returns></returns>
    [HttpPut("RemoveTrainingSchedule")]
    public async Task<object> RemoveTrainingSchedule(List<long> list)
    {
        var requestJWT = Request.Headers["Authorization"];
        TokenModel obj = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenModel>(client.GetTokenJson(requestJWT).Result);
        return new JsonResult(await lessonSchedule.RemoveTrainingSchedule(db,obj,list));
    }
    /// <summary>
    /// 获取培训计划内容
    /// </summary>
    /// <param name="planId"></param>
    /// <returns></returns>
    [HttpGet("GetPlanContentById")]
    public object GetPlanContentById(long planId)
    {
        return new JsonResult(lessonSchedule.GetPlanContentById(db,planId));
    }

    [HttpGet("AutoArrangeSchedule")]
    public object AutoArrangeSchedule(long planId)
    {
        return new JsonResult(lessonSchedule.AutoArrangeSchedule(db, planId));
    }

    #endregion

    #region 培训计划下查询

    /// <summary>
    /// 从培训计划下查询课程表内容
    /// </summary>
    /// <param name="planID"></param>
    /// <param name="trainingType"></param>
    /// <param name="startDate"></param>
    /// <returns></returns>
    [HttpGet("GetScheduleInfoFromPlan")]
    public object GetScheduleInfoFromPlan(long planID, string trainingType, string startDate)
    {
        return new JsonResult(lessonSchedule.GetScheduleInfoFromPlan(db,planID,trainingType,startDate));
    }

    #endregion
}

