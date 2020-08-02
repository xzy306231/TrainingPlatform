
using Microsoft.AspNetCore.Mvc;


/// <summary>
/// 我的训练
/// </summary>
[Route("trainingplan/v1")]
[ApiController]
public class MyTrainingTaskController : ControllerBase
{
    private readonly MyTrainingTask trainingTask;
    private readonly RabbitMQClient rabbit;
    private readonly pf_training_plan_v1Context db;
    public MyTrainingTaskController(MyTrainingTask trainingTask, RabbitMQClient rabbit, pf_training_plan_v1Context db)
    {
        this.trainingTask = trainingTask;
        this.rabbit = rabbit;
        this.db = db;
    }
    /// <summary>
    /// 我的训练下拉列表
    /// </summary>
    /// <param name="UserNumber">用户名</param>
    /// <returns></returns>
    [HttpGet("GetMyTrainTaskPlanList")]
    public JsonResult GetMyTrainTaskPlanList(string UserNumber)
    {
        return new JsonResult(trainingTask.GetMyTrainTaskPlanList(db,UserNumber));
    }
    /// <summary>
    /// 获取我的训练数据
    /// </summary>
    /// <param name="PlanID">计划ID</param>
    /// <param name="startTime">开始时间</param>
    /// <param name="endTime">结束时间</param>
    /// <param name="strKeyWord">搜索关键字</param>
    /// <param name="UserID">用户名</param>
    /// <param name="pageSize">记录数</param>
    /// <param name="pageIndex">页码</param>
    /// <returns></returns>
    [HttpGet("GetMyTrainingTask")]
    public JsonResult GetMyTrainingTask(long PlanID, string startTime, string endTime, string strKeyWord, string UserID, int pageSize, int pageIndex)
    {
        return new JsonResult(trainingTask.GetMyTrainingTask(db,PlanID, startTime, endTime, strKeyWord, UserID, pageSize, pageIndex));
    }
}
