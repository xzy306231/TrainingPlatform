using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Examination.BLL;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Examination.API.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route("examination/v1")]
    [ApiController]
    public class MyExaminationController : ControllerBase
    {
        private readonly IHttpClientHelper client;
        private readonly MyExamination myExamination;
        private readonly IConfiguration configuration;
        private readonly pf_examinationContext db;
        private readonly RabbitMQClient rabbit;
        public MyExaminationController(IHttpClientHelper client, MyExamination myExamination, IConfiguration configuration,pf_examinationContext db,RabbitMQClient rabbit)
        {
            this.client = client;
            this.myExamination = myExamination;
            this.configuration = configuration;
            this.db = db;
            this.rabbit = rabbit;
        }
        /// <summary>
        /// 获取我的考试列表信息
        /// </summary>
        /// <param name="keyWord">搜索关键字</param>
        /// <param name="strStatus">状态</param>
        /// <param name="StartTime">开始时间</param>
        /// <param name="EndTime">结束时间</param>
        /// <param name="UserNumber">用户账号</param>
        /// <param name="PageIndex">页码</param>
        /// <param name="PageSize">记录数</param>
        /// <returns></returns>
        [HttpGet("GetMyExamination")]
        public JsonResult GetMyExamination(string keyWord, string strStatus, string StartTime, string EndTime, string UserNumber, int PageIndex=1, int PageSize=10)
        {
            return new JsonResult(myExamination.GetMyExamination(db,keyWord, strStatus, StartTime, EndTime, UserNumber,PageIndex,PageSize));
        }

        /// <summary>
        /// 开始考试界面信息
        /// </summary>
        /// <param name="examid"></param>
        /// <returns></returns>
        [HttpGet("GetTheoryExamAndPaperInfoByExamID")]
        public JsonResult GetTheoryExamAndPaperInfoByExamID(long examid)
        {           
            return new JsonResult(myExamination.GetTheoryExamAndPaperInfoByExamID(db,examid));
        }

        /// <summary>
        /// 开始考试
        /// </summary>
        /// <param name="examinationid">考试管理ID</param>
        /// <param name="usernumber">账号</param>
        /// <returns></returns>
        [HttpPut("StartExamination")]
        public JsonResult StartExamination(long examinationid, string usernumber)
        {
            var requestJWT = Request.Headers["Authorization"];
            TokenModel obj = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenModel>(client.GetTokenJson(requestJWT).Result);
            return new JsonResult(myExamination.StartExamination(db,examinationid, usernumber,obj,client,rabbit));
        }

        /// <summary>
        /// 继续考试
        /// </summary>
        /// <param name="examinationid"></param>
        /// <param name="usernumber"></param>
        /// <returns></returns>
        [HttpGet("ContinueExamination")]
        public JsonResult ContinueExamination(long examinationid, string usernumber)
        {
            var requestJWT = Request.Headers["Authorization"];
            TokenModel obj = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenModel>(client.GetTokenJson(requestJWT).Result);
            return new JsonResult(myExamination.ContinueExamination(db,rabbit,examinationid, usernumber, obj));
        }

        /// <summary>
        /// 查看实践考试结果
        /// </summary>
        /// <param name="ExamID"></param>
        /// <param name="UserNumber"></param>
        /// <returns></returns>
        [HttpGet("GetTaskExamResultInfo")]
        public JsonResult GetTaskExamResultInfo(long ExamID, string UserNumber)
        {
            return new JsonResult(myExamination.GetTaskExamResultInfo(db,ExamID, UserNumber));
        }

        /// <summary>
        /// 获取学员试卷
        /// </summary>
        /// <param name="ExamID"></param>
        /// <param name="UserNumber"></param>
        /// <returns></returns>
        [HttpGet("GetStudentExamPaper")]
        public JsonResult GetStudentExamPaper(long ExamID, string UserNumber)
        {
            return new JsonResult(myExamination.GetStudentExamPaper(db,ExamID, UserNumber));
        }

        /// <summary>
        /// 获取学员作答查看界面信息
        /// </summary>
        /// <param name="ExamID"></param>
        /// <param name="UserNumber"></param>
        /// <returns></returns>
        [HttpGet("GetStuTheoryExamPaperResult")]
        public JsonResult GetStuTheoryExamPaperResult(long ExamID, string UserNumber)
        {
            return new JsonResult(myExamination.GetStuTheoryExamPaperResult(db,ExamID,UserNumber));
        }

        /// <summary>
        /// 学员提交试卷
        /// </summary>
        /// <param name="answerLogList"></param>
        /// <returns></returns>
        [HttpPost("StuSubmitExamPaper")]
        public async Task<JsonResult> StuSubmitExamPaper(AnswerLogList answerLogList)
        {
            var requestJWT = Request.Headers["Authorization"];
            TokenModel obj = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenModel>(client.GetTokenJson(requestJWT).Result);
            return new JsonResult(await myExamination.StuSubmitExamPaper(db,rabbit,answerLogList, obj));
        }

        /// <summary>
        /// 学员作答
        /// </summary>
        /// <param name="RecordID"></param>
        /// <param name="ItemID"></param>
        /// <param name="AnswerResult"></param>
        /// <returns></returns>
        [HttpPost("StuAnswerQuestion")]
        public async Task<JsonResult> StuAnswerQuestion(long RecordID, long ItemID, string AnswerResult)
        {
            return new JsonResult(await myExamination.StuAnswerQuestion(db,RecordID,ItemID,AnswerResult));
        }

        /// <summary>
        /// 学员提交试卷
        /// </summary>
        /// <param name="RecordID">记录ID</param>
        /// <returns></returns>
        [HttpPut("SubmitStudentTestPaper")]
        public async Task<JsonResult> SubmitStudentTestPaper(long RecordID)
        {
            var requestJWT = Request.Headers["Authorization"];
            TokenModel obj = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenModel>(client.GetTokenJson(requestJWT).Result);
            return new JsonResult(await myExamination.SubmitStudentTestPaper(db,rabbit,RecordID, obj));
        }
    
    }
}