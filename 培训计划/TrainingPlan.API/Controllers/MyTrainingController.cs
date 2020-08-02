using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

/// <summary>
/// 我的培训
/// </summary>
[Route("trainingplan/v1")]
[ApiController]
public class MyTrainingController : ControllerBase
{
    private readonly IHttpClientHelper client;
    private readonly MyTraining myTraining;
    private readonly IConfiguration configuration;
    private readonly RabbitMQClient rabbit;
    private readonly pf_training_plan_v1Context db;
    public MyTrainingController(IHttpClientHelper client, MyTraining myTraining, IConfiguration configuration, RabbitMQClient rabbit, pf_training_plan_v1Context db)
    {
        this.client = client;
        this.myTraining = myTraining;
        this.configuration = configuration;
        this.rabbit = rabbit;
        this.db = db;
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
    public JsonResult GetMyTraining(string strStatus, string planName, string startTime, string endTime, string UserID, int pageIndex = 1, int pageSize = 10)
    {
        var requestJWT = Request.Headers["Authorization"];
        TokenModel obj = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenModel>(client.GetTokenJson(requestJWT).Result);
        return new JsonResult(myTraining.GetMyTraining(db,strStatus, planName, startTime, endTime, UserID, obj, pageIndex, pageSize));
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
        return new JsonResult(myTraining.GetStuTrainingPlanContent(db,PlanID, UserID));
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
        return new JsonResult(myTraining.GetCourseInfoByID(db,configuration,CourseID, RecordID));
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
        return new JsonResult(myTraining.GetLearningCourseStruct(db,courseid, recordID));
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
        return new JsonResult(myTraining.CheckLearningCondition(db,ContentID, UserID, client));
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
        return new JsonResult(myTraining.Add_LearningRecord(db,rabbit,ContentID, UserID, obj));
    }

    /// <summary>
    /// 查看培训计划下的所有学员
    /// </summary>
    /// <param name="PlanID"></param>
    /// <returns></returns>
    [HttpGet("GetStuFromPlan")]
    public JsonResult GetStuFromPlan(long PlanID)
    {
        string str = configuration["DicUrl"];
        //部门
        string strDepartment = client.GetRequest(str + "department_type").Result;
        DicModel dicDepartment = Newtonsoft.Json.JsonConvert.DeserializeObject<DicModel>(strDepartment);

        return new JsonResult(myTraining.GetStuFromPlan(db,configuration,PlanID, dicDepartment));
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
        return new JsonResult(myTraining.LearningCoursewareByStructNodeID(db,configuration,RecordID, NodeID));
    }
    /// <summary>
    /// 学习自定义课件
    /// </summary>
    /// <param name="courseid"></param>
    /// <param name="recordId"></param>
    /// <param name="nodeId"></param>
    /// <returns></returns>
    [HttpGet("LearningCustomCourseWare")]
    public JsonResult LearningCustomCourseWare(long courseid, long recordId, long nodeId)
    {
        return new JsonResult(myTraining.LearningCustomCourseWare(db,courseid, recordId, nodeId));
    }
    /// <summary>
    /// 创建或修改课程节点学习记录
    /// </summary>
    /// <param name="RecordID">记录ID</param>
    /// <param name="SrcID">课程ID</param>
    /// <param name="StructID">节点ID</param>
    /// <param name="NodeStatus">节点状态</param>
    /// <param name="pageNum">自定义课件页码</param>
    /// <param name="LearningTime">学习时长（单位：秒）</param>
    /// <returns></returns>
    [HttpPost("Add_CourseNodeLearningStatus")]
    public JsonResult Add_CourseNodeLearningStatus(long RecordID, long SrcID, long StructID, string NodeStatus, int pageNum, int LearningTime)
    {
        var requestJWT = Request.Headers["Authorization"];
        TokenModel obj = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenModel>(client.GetTokenJson(requestJWT).Result);
        return new JsonResult(myTraining.Add_CourseNodeLearningStatus(db,rabbit,RecordID, SrcID, StructID, NodeStatus, pageNum, LearningTime, obj, client));
    }

}
