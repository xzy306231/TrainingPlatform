using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PaperQuestions.BLL;

namespace PaperQuestions.API.Controllers
{
    /// <summary>
    /// 知识点组卷
    /// </summary>
    [Route("paperquestion/v1")]
    [ApiController]
    public class KnowledgePaperController : ControllerBase
    {
        private readonly IHttpClientHelper _client;
        private readonly ExaminationPapers papers;
        private readonly pf_exam_paper_questionsContext db;
        private readonly RabbitMQClient rabbit;
        public KnowledgePaperController(IHttpClientHelper client, ExaminationPapers papers, pf_exam_paper_questionsContext db,RabbitMQClient rabbit)
        {
            _client = client;
            this.papers = papers;
            this.db = db;
            this.rabbit = rabbit;
        }

        [HttpPost("GetKnowledgeQuestions")]
        public JsonResult GetKnowledgeQuestions(QuestionsQueryParameter parameter)
        {
            return new JsonResult(papers.GetKnowledgeQuestions(db,parameter));
        }

        [HttpPost("AddQuestionToBasket")]
        public async Task<object> AddQuestionToBasket(List<QuestionAttribute> idList)
        {
            var requestJWT = Request.Headers["Authorization"];
            TokenModel obj = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenModel>(_client.GetTokenJson(requestJWT).Result);
            return new JsonResult(await papers.AddQuestionToBasket(db,rabbit,idList, obj));
        }

        [HttpPut("CancleSelectedQuestion")]
        public JsonResult CancleSelectedQuestion(string userNumber, long questionId)
        {
            var requestJWT = Request.Headers["Authorization"];
            TokenModel obj = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenModel>(_client.GetTokenJson(requestJWT).Result);
            return new JsonResult( papers.CancleSelectedQuestion(db,rabbit,userNumber, questionId,obj));
        }
    }
}