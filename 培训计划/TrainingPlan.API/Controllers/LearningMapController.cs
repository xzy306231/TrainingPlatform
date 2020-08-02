using Microsoft.AspNetCore.Mvc;

/// <summary>
/// 学习地图
/// </summary>
[Route("trainingplan/v1")]
[ApiController]
public class LearningMapController : ControllerBase
{
    private readonly LearningMap map;
    private readonly IHttpClientHelper client;
    private readonly RabbitMQClient rabbit;
    private readonly pf_training_plan_v1Context db;
    public LearningMapController(LearningMap map,IHttpClientHelper client, RabbitMQClient rabbit, pf_training_plan_v1Context db)
    {
        this.map = map;
        this.client = client;
        this.rabbit = rabbit;
        this.db = db;
    }

    [HttpGet("GetLearningMapInfo")]
    public JsonResult GetLearningMapInfo(string userNumber,long userId)
    {
        return new JsonResult(map.GetLearningMapInfo(db,userNumber,userId,client));
    }
}

