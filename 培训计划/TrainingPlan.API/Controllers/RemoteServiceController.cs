using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// 远程服务
/// </summary>
[Route("trainingplan/v1")]
[ApiController]
public class RemoteServiceController : ControllerBase
{
    private readonly RemoteService remoteService;
    private readonly RabbitMQClient rabbit;
    private readonly pf_training_plan_v1Context db;
    public RemoteServiceController(RemoteService remoteService, RabbitMQClient rabbit, pf_training_plan_v1Context db)
    {
        this.remoteService = remoteService;
        this.rabbit = rabbit;
        this.db = db;
    }
    /// <summary>
    /// 更新学生考试得分
    /// </summary>
    /// <param name="examResults"></param>
    /// <returns></returns>
    [HttpPut("UpdateStuScore")]
    public JsonResult UpdateStuScore(List<ExamResult> examResults)
    {
        return new JsonResult(remoteService.UpdateStuScore(db,examResults));
    }

    /// <summary>
    ///  更新培训计划进度
    /// </summary>
    /// <param name="planProgresses"></param>
    /// <returns></returns>
    [HttpPut("UpdatePlanProgress")]
    public JsonResult UpdatePlanProgress(List<PlanProgress> planProgresses)
    {
        return new JsonResult(remoteService.UpdatePlanProgress(db,planProgresses));
    }

    /// <summary>
    /// 更新考试完成率
    /// </summary>
    /// <param name="examFinishRate"></param>
    /// <returns></returns>
    [HttpPut("UpdateExamFinishRate")]
    public JsonResult UpdateExamFinishRate(ExamFinishRate examFinishRate)
    {
        return new JsonResult(remoteService.UpdateExamFinishRate(db,examFinishRate));
    }
}
