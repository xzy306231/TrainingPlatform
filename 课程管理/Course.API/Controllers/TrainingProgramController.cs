using Course.BLL;
using Course.Model;
using Microsoft.AspNetCore.Mvc;

namespace Course.API.Controllers
{
    /// <summary>
    /// 培训大纲控制器
    /// </summary>
    [Route("course/v1")]
    [ApiController]
    public class TrainingProgramController : ControllerBase
    {
        private readonly IHttpClientHelper client;
        private readonly TrainingProgram program;
        private readonly pf_course_manage_v1Context db;
        private readonly RabbitMQClient rabbit;
        /// <summary>
        /// 培训大纲
        /// </summary>
        /// <param name="client"></param>
        public TrainingProgramController(IHttpClientHelper client, TrainingProgram program, pf_course_manage_v1Context db, RabbitMQClient rabbit)
        {
            this.client = client;
            this.program = program;
            this.db = db;
            this.rabbit = rabbit;
        }
        /// <summary>
        /// 获取培训大纲
        /// </summary>
        /// <param name="planeType">机型</param>
        /// <param name="TrainType">训练类别</param>
        /// <param name="TrainName">名称</param>
        /// <param name="PageIndex">训练类别</param>
        /// <param name="PageSize">名称</param>
        /// <returns></returns>
        [HttpGet("GetTrainingProgram")]
        public JsonResult GetTrainingProgram(string planeType, string TrainType, string TrainName, int PageIndex=1, int PageSize=10)
        {
            return new JsonResult(program.GetTrainingProgram(db,planeType, TrainType, TrainName, PageIndex, PageSize));
        }
        /// <summary>
        /// 根据ID获取某一个培训大纲
        /// </summary>
        /// <param name="TrainingProgramID"></param>
        /// <returns></returns>
        [HttpGet("GetTrainingProgramByID")]
        public JsonResult GetTrainingProgramByID(long TrainingProgramID)
        {
            return new JsonResult(program.GetTrainingProgramByID(db,TrainingProgramID));
        }
        /// <summary>
        /// 删除培训大纲
        /// </summary>
        /// <param name="TrainingProgramID">培训大纲ID</param>
        /// <returns></returns>
        [HttpDelete("Delete_TrainingProgram")]
        public JsonResult Delete_TrainingProgram(long TrainingProgramID)
        {
            var requestJWT = Request.Headers["Authorization"];
            TokenModel obj = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenModel>(client.GetTokenJson(requestJWT).Result);
            return new JsonResult(program.Delete_TrainingProgram(db,rabbit,TrainingProgramID,obj));
        }

        /// <summary>
        /// 修改训练大纲
        /// </summary>
        /// <param name="objModel"></param>
        /// <returns></returns>
        [HttpPut("Update_TrainingProgram")]
        public JsonResult Update_TrainingProgram(TrainingProgramModel objModel)
        {
            var requestJWT = Request.Headers["Authorization"];
            TokenModel obj = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenModel>(client.GetTokenJson(requestJWT).Result);
            return new JsonResult(program.Update_TrainingProgram(db,rabbit,objModel,obj));
        }

        /// <summary>
        /// 创建训练大纲
        /// </summary>
        /// <param name="objModel"></param>
        /// <returns></returns>
        [HttpPost("Add_TrainingProgram")]
        public JsonResult Add_TrainingProgram(TrainingProgramModel objModel)
        {
            var requestJWT = Request.Headers["Authorization"];
            TokenModel obj = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenModel>(client.GetTokenJson(requestJWT).Result);
            return new JsonResult(program.Add_TrainingProgram(db,rabbit,objModel,obj));
        }
    }
}