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
    /// 试卷篮
    /// </summary>
    [Route("paperquestion/v1")]
    [ApiController]
    public class BasketQuestionsController : ControllerBase
    {

        private readonly IHttpClientHelper client;
        private readonly ExaminationPapers papers;
        private readonly pf_exam_paper_questionsContext db;
        private readonly RabbitMQClient rabbit;
        public BasketQuestionsController(IHttpClientHelper client, ExaminationPapers papers, pf_exam_paper_questionsContext db,RabbitMQClient rabbit)
        {
            this.client = client;
            this.papers = papers;
            this.db = db;
            this.rabbit = rabbit;
        }
        /// <summary>
        /// 获取试卷篮试题
        /// </summary>
        /// <param name="userNumber"></param>
        /// <returns></returns>
        [HttpGet("GetBasketQuestions")]
        public JsonResult GetBasketQuestions(string userNumber)
        {
            return new JsonResult(papers.GetBasketQuestions(db,userNumber));
        }
        /// <summary>
        /// 试卷篮试题排序
        /// </summary>
        /// <param name="questionsSort"></param>
        /// <returns></returns>
        [HttpPut("BasketQuestionsSort")]
        public async Task<JsonResult> BasketQuestionsSort(List<QuestionTypeScore> questionsSort)
        {
            var requestJWT = Request.Headers["Authorization"];
            TokenModel obj = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenModel>(client.GetTokenJson(requestJWT).Result);
            return new JsonResult(await papers.BasketQuestionsSort(db,rabbit,questionsSort,obj));
        }
        /// <summary>
        /// 题型排序
        /// </summary>
        /// <param name="questionsSort"></param>
        /// <returns></returns>
        [HttpPut("BasketQuestionTypeSort")]
        public JsonResult BasketQuestionTypeSort(List<QuestionTypeScore> questionsSort)
        {
            
            var requestJWT = Request.Headers["Authorization"];
            TokenModel obj = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenModel>(client.GetTokenJson(requestJWT).Result);
            return new JsonResult(papers.BasketQuestionTypeSort(db,rabbit,questionsSort, obj));
        }

        /// <summary>
        /// 删除试卷篮试题
        /// </summary>
        /// <param name="basketID"></param>
        /// <returns></returns>
        [HttpPut("RemoveQuestionFromBasket")]
        public async Task<JsonResult> RemoveQuestionFromBasket(long basketID)
        {
            var requestJWT = Request.Headers["Authorization"];
            TokenModel obj = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenModel>(client.GetTokenJson(requestJWT).Result);
            return new JsonResult(await papers.RemoveQuestionFromBasket(db,rabbit,basketID,obj));
        }
        /// <summary>
        /// 按照题型删除试题
        /// </summary>
        /// <param name="questionType"></param>
        /// <returns></returns>
        [HttpPut("RemoveQuestionByQuestionType")]
        public JsonResult RemoveQuestionByQuestionType(string questionType)
        {
            var requestJWT = Request.Headers["Authorization"];
            TokenModel obj = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenModel>(client.GetTokenJson(requestJWT).Result);
            return new JsonResult( papers.RemoveQuestionByQuestionType(db,rabbit,questionType,obj));
        }
        /// <summary>
        /// 设置试题分值
        /// </summary>
        /// <param name="typeScoreList"></param>
        /// <returns></returns>
        [HttpPut("SetQuestionScore")]
        public async Task<JsonResult> SetQuestionScore(List<QuestionTypeScore> typeScoreList)
        {
            var requestJWT = Request.Headers["Authorization"];
            TokenModel obj = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenModel>(client.GetTokenJson(requestJWT).Result);
            return new JsonResult(await papers.SetQuestionScore(db,rabbit,typeScoreList, obj));
        }
        /// <summary>
        /// 试卷分析
        /// </summary>
        /// <param name="userNumber"></param>
        /// <returns></returns>
        [HttpGet("GetTestPaperAnalyze")]
        public JsonResult GetTestPaperAnalyze(string userNumber)
        {
            return new JsonResult( papers.GetTestPaperAnalyze(db,userNumber));
        }
        /// <summary>
        /// 保存试卷篮试题
        /// </summary>
        /// <param name="paperInfo"></param>
        /// <returns></returns>
        [HttpPost("SaveBasketQuestions")]
        public async Task<object> SaveBasketQuestions(Paper paperInfo)
        {
            var requestJWT = Request.Headers["Authorization"];
            TokenModel obj = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenModel>(client.GetTokenJson(requestJWT).Result);
            return new JsonResult(await papers.SaveBasketQuestions(db,rabbit,paperInfo, obj));
        }
    }
}