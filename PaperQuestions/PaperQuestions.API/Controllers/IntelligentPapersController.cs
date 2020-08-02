using Microsoft.AspNetCore.Mvc;
using PaperQuestions.BLL;

namespace PaperQuestions.API.Controllers
{
    /// <summary>
    /// 智能组卷
    /// </summary>
    [Route("paperquestion/v1")]
    [ApiController]
    public class IntelligentPapersController : ControllerBase
    {
        private readonly IHttpClientHelper client;
        private readonly ExaminationPapers papers;
        private readonly pf_exam_paper_questionsContext db;
        private readonly RabbitMQClient rabbit;
        public IntelligentPapersController(IHttpClientHelper client, ExaminationPapers papers, pf_exam_paper_questionsContext db,RabbitMQClient rabbit)
        {
            this.client = client;
            this.papers = papers;
            this.db = db;
            this.rabbit = rabbit;
        }

        /// <summary>
        /// 根据题型获取相应的数量
        /// </summary>
        /// <param name="queryParameter"></param>
        /// <returns></returns>
        [HttpPost("GetQuestionTypeCount")]
        public object GetQuestionTypeCount(IntelligentParameter queryParameter)
        {
            return new JsonResult(papers.GetQuestionTypeCount(db,queryParameter));
        }
        /// <summary>
        /// 获取题型数量
        /// </summary>
        /// <returns></returns>
        [HttpPost("GetAllQuestionTypeCount")]
        public object GetAllQuestionTypeCount(IntelligentParameter queryParameter)
        {
            return new JsonResult(papers.GetAllQuestionTypeCount(db,queryParameter));
        }
        /// <summary>
        /// 智能组卷
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        [HttpPost("IntelligentComposeTestPaper")]
        public object IntelligentComposeTestPaper(IntelligentParameter parameter)
        {
            var requestJWT = Request.Headers["Authorization"];
            TokenModel obj = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenModel>(client.GetTokenJson(requestJWT).Result);
            return new JsonResult(papers.IntelligentComposeTestPaper(db,rabbit,parameter,obj));
        }
    }
}