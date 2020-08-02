using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

/// <summary>
/// 课程管理
/// </summary>
[Route("trainingplan/v1")]
[ApiController]
public class CourseController : ControllerBase
{
    private readonly CourseManagement course;
    private readonly IConfiguration configuration;
    private readonly RabbitMQClient rabbit;
    private readonly pf_training_plan_v1Context db;
    public CourseController(CourseManagement course, IConfiguration configuration, RabbitMQClient rabbit, pf_training_plan_v1Context db)
    {
        this.course = course;
        this.configuration = configuration;
        this.rabbit = rabbit;
        this.db = db;
    }

    [HttpGet("GetCourseStruct")]
    public JsonResult GetCourseStruct(long courseid)
    {
        return new JsonResult(course.GetCourseStruct(db,courseid));
    }

    /// <summary>
    /// 根据ID获取课程信息
    /// </summary>
    /// <param name="ID"></param>
    /// <returns></returns>
    [HttpGet("GetCourseInfoByID")]
    public JsonResult GetCourseInfoByID(long ID)
    {
        return new JsonResult(course.GetCourseInfoByID(db,configuration,ID));
    }
}
