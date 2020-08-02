using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PaperQuestions.BLL;

namespace PaperQuestions.API.Controllers
{
    /// <summary>
    /// 我的组卷
    /// </summary>
    [Route("paperquestion/v1")]
    [ApiController]
    public class MyPaperController : ControllerBase
    {
        private readonly IHttpClientHelper _client;
        private readonly ExaminationPapers papers;
        private readonly pf_exam_paper_questionsContext db;
        private readonly RabbitMQClient rabbit;
        public MyPaperController(IHttpClientHelper client, ExaminationPapers papers, pf_exam_paper_questionsContext db,RabbitMQClient rabbit)
        {
            _client = client;
            this.papers = papers;
            this.db = db;
            this.rabbit = rabbit;
        }

        /// <summary>
        /// 我的组卷
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="keyWord"></param>
        /// <param name="userNumber"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet("GetMyTestPaper")]
        public JsonResult GetMyTestPaper(string startTime, string endTime, string keyWord, string userNumber, int pageIndex = 1, int pageSize = 10)
        {
            return new JsonResult(papers.GetMyTestPaper(db,startTime, endTime, keyWord, userNumber, pageIndex, pageSize));
        }
        /// <summary>
        /// 分享我的组卷
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("ShareMyTestPaper")]
        public async Task<object> ShareMyTestPaper(long id)
        {
            var requestJWT = Request.Headers["Authorization"];
            TokenModel obj = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenModel>(_client.GetTokenJson(requestJWT).Result);
            return new JsonResult(await papers.ShareMyTestPaper(db,rabbit,id,obj));
        }
        /// <summary>
        /// 取消分享我的组卷
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("CancleShareMyTestPaper")]
        public async Task<object> CancleShareMyTestPaper(long id)
        {
            return new JsonResult(await papers.CancleShareMyTestPaper(db,id));
        }
        /// <summary>
        /// 删除我的组卷
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("RemoveTestPaper")]
        public async Task<object> RemoveTestPaper(long id)
        {
            var requestJWT = Request.Headers["Authorization"];
            TokenModel obj = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenModel>(_client.GetTokenJson(requestJWT).Result);
            return new JsonResult(await papers.RemoveTestPaper(db,rabbit,id,obj));
        }
        /// <summary>
        /// 从试卷中删除试题
        /// </summary>
        /// <param name="id"></param>
        /// <param name="paperid"></param>
        /// <returns></returns>
        [HttpPut("RemoveQuestionFromTestPaper")]
        public async Task<object> RemoveQuestionFromTestPaper(long id, long paperid)
        {
            var requestJWT = Request.Headers["Authorization"];
            TokenModel obj = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenModel>(_client.GetTokenJson(requestJWT).Result);
            return new JsonResult(await papers.RemoveQuestionFromTestPaper(db,rabbit,id, paperid,obj));
        }
        /// <summary>
        /// 按题型删除试题
        /// </summary>
        /// <param name="questionType"></param>
        /// <returns></returns>
        [HttpPut("RemoveQuestionsByType")]
        public async Task<object> RemoveQuestionsByType(string questionType)
        {
            var requestJWT = Request.Headers["Authorization"];
            TokenModel obj = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenModel>(_client.GetTokenJson(requestJWT).Result);
            return new JsonResult(await papers.RemoveQuestionsByType(db,rabbit,questionType,obj));
        }
        /// <summary>
        /// 设置分值
        /// </summary>
        /// <param name="typeScoreList"></param>
        /// <returns></returns>
        [HttpPut("SetQuestionsScore")]
        public async Task<object> SetQuestionsScore(List<QuestionTypeScore> typeScoreList)
        {
            var requestJWT = Request.Headers["Authorization"];
            TokenModel obj = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenModel>(_client.GetTokenJson(requestJWT).Result);
            return new JsonResult(await papers.SetQuestionsScore(db,rabbit,typeScoreList,obj));
        }
        /// <summary>
        /// 试题排序
        /// </summary>
        /// <param name="questionsSort"></param>
        /// <returns></returns>
        [HttpPut("MyTestPaperQuestionsSort")]
        public async Task<object> MyTestPaperQuestionsSort(List<QuestionTypeScore> questionsSort)
        {
            var requestJWT = Request.Headers["Authorization"];
            TokenModel obj = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenModel>(_client.GetTokenJson(requestJWT).Result);
            return new JsonResult(await papers.MyTestPaperQuestionsSort(db,rabbit,questionsSort,obj));
        }
        /// <summary>
        /// 按题型排序
        /// </summary>
        /// <param name="questionsSort"></param>
        /// <returns></returns>
        [HttpPut("MyTestPaperQuestionTypeSort")]
        public async Task<object> MyTestPaperQuestionTypeSort(List<QuestionTypeScore> questionsSort)
        {
            var requestJWT = Request.Headers["Authorization"];
            TokenModel obj = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenModel>(_client.GetTokenJson(requestJWT).Result);
            return new JsonResult(await papers.MyTestPaperQuestionTypeSort(db,rabbit,questionsSort, obj));
        }
        /// <summary>
        /// 获取我的组卷试卷信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("GetMyTestPaperQuestions")]
        public JsonResult GetMyTestPaperQuestions(long id)
        {
            return new JsonResult(papers.GetMyTestPaperQuestions(db,id));
        }

        /// <summary>
        /// 编辑试卷标题
        /// </summary>
        /// <param name="paperid"></param>
        /// <param name="paperTitle"></param>
        /// <returns></returns>
        [HttpPut("EditTestPaperTitle")]
        public async Task<object> EditTestPaperTitle(long paperid, string paperTitle)
        {
            var requestJWT = Request.Headers["Authorization"];
            TokenModel obj = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenModel>(_client.GetTokenJson(requestJWT).Result);
            return new JsonResult(await papers.EditTestPaperTitle(db,rabbit,paperid,paperTitle,obj));
        }
        /// <summary>
        /// 试卷分析
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("GetMyTestPaperAnalyze")]
        public object GetMyTestPaperAnalyze(long id)
        {
            return new JsonResult(papers.GetMyTestPaperAnalyze(db,id));
        }
    }
}