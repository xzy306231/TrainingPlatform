using Microsoft.AspNetCore.Mvc;
using PaperQuestions.BLL;

namespace PaperQuestions.API.Controllers
{
    /// <summary>
    /// 试卷库
    /// </summary>
    [Route("paperquestion/v1")]
    [ApiController]
    public class PapersLibraryController : ControllerBase
    {
        private readonly IHttpClientHelper _client;
        private readonly ExaminationPapers _papers;
        private readonly pf_exam_paper_questionsContext db;
        private readonly RabbitMQClient rabbit;
        public PapersLibraryController(IHttpClientHelper client, ExaminationPapers papers, pf_exam_paper_questionsContext db,RabbitMQClient rabbit)
        {
            _client = client;
            _papers = papers;
            this.db = db;
            this.rabbit = rabbit;
        }

        /// <summary>
        /// 获取试卷库列表
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="keyWord"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet("GetTestPaper")]
        public object GetTestPaper(string startTime, string endTime, string keyWord, int pageIndex = 1, int pageSize = 10)
        {
            return new JsonResult(_papers.GetTestPaper(db,startTime, endTime, keyWord, pageIndex, pageSize));
        }
        /// <summary>
        /// 使用此卷
        /// </summary>
        /// <param name="paperid"></param>
        /// <returns></returns>
        [HttpPost("ReuseTestPaper")]
        public object ReuseTestPaper(long paperid)
        {
            var requestJWT = Request.Headers["Authorization"];
            TokenModel obj = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenModel>(_client.GetTokenJson(requestJWT).Result);
            return new JsonResult(_papers.ReuseTestPaper(db,rabbit,paperid,obj));
        }
        
       
    }
}