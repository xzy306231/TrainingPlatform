using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PaperQuestions.BLL;

namespace PaperQuestions.API.Controllers
{
    /// <summary>
    /// 题库管理
    /// </summary>
    [Route("paperquestion/v1")]
    [ApiController]
    public class QuestionsController : ControllerBase
    {
        private readonly IHttpClientHelper client;
        private readonly Questions questions;
        private readonly pf_exam_paper_questionsContext db;
        private readonly RabbitMQClient rabbit;
        public QuestionsController(IHttpClientHelper client, Questions questions, pf_exam_paper_questionsContext db,RabbitMQClient rabbit)
        {
            this.client = client;
            this.questions = questions;
            this.db = db;
            this.rabbit = rabbit;
        }
        /// <summary>
        /// 查询试题
        /// </summary>
        /// <param name="questionType"></param>
        /// <param name="complexity"></param>
        /// <param name="publishFlag"></param>
        /// <param name="keyWord"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet("GetQuestions")]
        public JsonResult GetQuestions(string questionType, string complexity, string publishFlag, string keyWord, bool IsAsc, string FieldName, int pageIndex = 1, int pageSize = 10)
        {
            return new JsonResult(questions.GetQuestions(db,questionType, complexity, publishFlag, keyWord,IsAsc,FieldName, pageIndex, pageSize));
        }
        /// <summary>
        /// 删除试题
        /// </summary>
        /// <param name="qusetionIDList"></param>
        /// <returns></returns>
        [HttpDelete("DeleteQuestions")]
        public JsonResult DeleteQuestions(QusetionIDList qusetionIDList)
        {
            var requestJWT = Request.Headers["Authorization"];
            TokenModel obj = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenModel>(client.GetTokenJson(requestJWT).Result);
            return new JsonResult(questions.DeleteQuestions(db,rabbit,qusetionIDList,obj));
        }
        /// <summary>
        /// 批量发布
        /// </summary>
        /// <param name="qusetionIDList"></param>
        /// <returns></returns>
        [HttpPut("PublishBatchQuestions")]
        public JsonResult PublishBatchQuestions(QusetionIDList qusetionIDList)
        {
            var requestJWT = Request.Headers["Authorization"];
            TokenModel obj = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenModel>(client.GetTokenJson(requestJWT).Result);
            return new JsonResult(questions.PublishBatchQuestions(db,rabbit,qusetionIDList,obj));
        }
        /// <summary>
        /// 根据ID查找试题信息
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet("GetQuestionByID")]
        public JsonResult GetQuestionByID(long Id)
        {
            return new JsonResult(questions.GetQuestionByID(db,Id));
        }
        /// <summary>
        /// 创建试题
        /// </summary>
        /// <param name="questionInfo"></param>
        /// <returns></returns>
        [HttpPost("CreateQuestions")]
        public JsonResult CreateQuestions(Question questionInfo)
        {
            var requestJWT = Request.Headers["Authorization"];
            TokenModel obj = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenModel>(client.GetTokenJson(requestJWT).Result);
            return new JsonResult(questions.CreateQuestion(db,rabbit,questionInfo, obj));
        }
        /// <summary>
        /// 修改试题
        /// </summary>
        /// <param name="questionInfo"></param>
        /// <returns></returns>
        [HttpPut("UpdateQuestion")]
        public JsonResult UpdateQuestion(Question questionInfo)
        {
            var requestJWT = Request.Headers["Authorization"];
            TokenModel obj = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenModel>(client.GetTokenJson(requestJWT).Result);
            return new JsonResult(questions.UpdateQuestion(db,rabbit,questionInfo,obj));
        }

        /// <summary>
        /// 试题导入
        /// </summary>
        /// <param name="questionmation"></param>
        /// <returns></returns>
        [HttpPost("ImportQuestions")]
        public JsonResult ImportQuestions(Questionmation questionmation)
        {
            var requestJWT = Request.Headers["Authorization"];
            TokenModel obj = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenModel>(client.GetTokenJson(requestJWT).Result);
            return new JsonResult(questions.ImportQuestions(db,questionmation,obj));
        }
        /// <summary>
        /// 未审核
        /// </summary>
        /// <param name="keyWord"></param>
        /// <param name="IsAsc"></param>
        /// <param name="FieldName"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet("GetNotApprovalQuestions")]
        public JsonResult GetNotApprovalQuestions(string keyWord, bool IsAsc, string FieldName, int pageIndex = 1, int pageSize = 10)
        {
            return new JsonResult(questions.GetNotApprovalQuestions(db,keyWord,IsAsc,FieldName,pageIndex,pageSize));
        }
        /// <summary>
        /// 已审核
        /// </summary>
        /// <param name="keyWord"></param>
        /// <param name="IsAsc"></param>
        /// <param name="FieldName"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet("GetApprovaledQuestions")]
        public JsonResult GetApprovaledQuestions(string keyWord, bool IsAsc, string FieldName, int pageIndex = 1, int pageSize = 10)
        {
            return new JsonResult(questions.GetApprovaledQuestions(db,keyWord, IsAsc, FieldName, pageIndex, pageSize));
        }
        /// <summary>
        /// 审核
        /// </summary>
        /// <param name="questionApprovals"></param>
        /// <returns></returns>
        [HttpPut("ApprovalQuestions")]
        public async Task<JsonResult> ApprovalQuestions(List<PaperQuestionApproval> questionApprovals)
        {
            var requestJWT = Request.Headers["Authorization"];
            TokenModel obj = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenModel>(client.GetTokenJson(requestJWT).Result);
            return new JsonResult(await questions.ApprovalQuestions(db,rabbit,questionApprovals,obj));
        }
    }
}