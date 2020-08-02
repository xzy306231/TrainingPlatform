
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// 统计分析数据
/// </summary>
[Route("trainingplan/v1")]
[ApiController]
public class StatisticDataController : ControllerBase
{
    private readonly StatisticData statisticData;
    private readonly RabbitMQClient rabbit;
    private readonly pf_training_plan_v1Context db;
    public StatisticDataController(StatisticData statisticData, RabbitMQClient rabbit, pf_training_plan_v1Context db)
    {
        this.statisticData = statisticData;
        this.rabbit = rabbit;
        this.db = db;
    }
    #region 培训统计

    /// <summary>
    /// 获取当前正在进行的培训计划数量
    /// </summary>
    /// <returns></returns>
    [HttpGet("GetCurrentTrainingPlanCount")]
    public JsonResult GetCurrentTrainingPlanCount()
    {
        return new JsonResult(statisticData.GetCurrentTrainingPlanCount(db));
    }

    /// <summary>
    /// 获取培训计划信息
    /// </summary>
    /// <returns></returns>
    [HttpGet("GetTrainingPlanInfo")]
    public JsonResult GetTrainingPlanInfo()
    {
        return new JsonResult(statisticData.GetTrainingPlanInfo(db));
    }

    /// <summary>
    /// 按年份查找培训计划中人数
    /// </summary>
    /// <param name="year"></param>
    /// <returns></returns>
    [HttpGet("GetTrainingPlanInfoByYear")]
    public JsonResult GetTrainingPlanInfoByYear(int year)
    {
        return new JsonResult(statisticData.GetTrainingPlanInfoByYear(db,year));
    }

    /// <summary>
    ///  获取培训计划完成率
    /// </summary>
    /// <param name="IsAsc"></param>
    /// <param name="FeildName"></param>
    /// <returns></returns>
    [HttpGet("GetTrainingPlanFinishRate")]
    public JsonResult GetTrainingPlanFinishRate(bool IsAsc = false, string FeildName = "")
    {
        return new JsonResult(statisticData.GetTrainingPlanFinishRate(db,IsAsc, FeildName));
    }

    #endregion

    #region 理论教学

    /// <summary>
    /// 获取培训计划平均学习时长
    /// </summary>
    /// <returns></returns>
    [HttpGet("GetAvgLearningTime")]
    public JsonResult GetAvgLearningTime()
    {
        return new JsonResult(statisticData.GetAvgLearningTime(db));
    }

    /// <summary>
    /// 获取课程学习完成率
    /// </summary>
    /// <returns></returns>
    [HttpGet("GetCourseFinishRate")]
    public JsonResult GetCourseFinishRate(bool IsAsc, string FeildName)
    {
        return new JsonResult(statisticData.GetCourseFinishRate(db,IsAsc, FeildName));
    }

    /// <summary>
    /// 获取知识点掌握度
    /// </summary>
    /// <returns></returns>
    [HttpGet("GetKnowledgeDegree")]
    public JsonResult GetKnowledgeDegree()
    {
        return new JsonResult(statisticData.GetKnowledgeDegree(db));
    }

    #endregion

}

