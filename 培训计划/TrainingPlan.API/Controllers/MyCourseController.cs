
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

/// <summary>
/// 我的课程
/// </summary>
[Route("trainingplan/v1")]
[ApiController]
public class MyCourseController : ControllerBase
{
    private readonly MyCourse myCourse;
    private readonly IConfiguration configuration;
    private readonly RabbitMQClient rabbit;
    private readonly pf_training_plan_v1Context db;
    public MyCourseController(MyCourse myCourse, IConfiguration configuration, RabbitMQClient rabbit, pf_training_plan_v1Context db)
    {
        this.myCourse = myCourse;
        this.configuration = configuration;
        this.rabbit = rabbit;
        this.db = db;
    }
    /// <summary>
    /// 获取我的培训计划下拉列表
    /// </summary>
    /// <param name="UserID">用户ID</param>
    /// <returns></returns>
    [HttpGet("GetMyTrainingPlanLList")]
    public JsonResult GetMyTrainingPlanLList(string UserID)
    {
        return new JsonResult(myCourse.GetMyTrainingPlanLList(db,UserID));
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
        return new JsonResult(myCourse.GetMyCourse(db,configuration,PlanID, startTime, endTime, strKeyWord, UserID, pageSize, pageIndex));
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
        return new JsonResult(myCourse.GetStuCourseLearningRecord(db,UserID, PlanID, CourseID));
    }

}
