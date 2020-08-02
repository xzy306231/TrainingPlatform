using Examination.BLL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace Examination.API.Controllers
{
    /// <summary>
    /// 阅卷管理
    /// </summary>
    [Route("examination/v1")]
    [ApiController]
    public class CorrectExamPaperController : ControllerBase
    {
        private readonly IHttpClientHelper client;
        private readonly CorrectExamPaper correct;
        private readonly IConfiguration configuration;
        private readonly RabbitMQClient rabbit;
        private readonly pf_examinationContext db;
        public CorrectExamPaperController(IHttpClientHelper client, CorrectExamPaper correct, IConfiguration configuration,RabbitMQClient rabbit, pf_examinationContext db)
        {
            this.client = client;
            this.correct = correct;
            this.configuration = configuration;
            this.rabbit = rabbit;
            this.db = db;
        }

        /// <summary>
        /// 获取阅卷信息
        /// </summary>
        /// <param name="UserNumber"></param>
        /// <param name="strStatus"></param>
        /// <param name="StartTime"></param>
        /// <param name="EndTime"></param>
        /// <param name="KeyWord"></param>
        /// <param name="PageIndex"></param>
        /// <param name="PageSize"></param>
        /// <returns></returns>
        [HttpGet("GetCorrectExamPaper")]
        public JsonResult GetCorrectExamPaper(string UserNumber, string strStatus, string StartTime, string EndTime, string KeyWord, int PageIndex=1, int PageSize=10)
        {
            return new JsonResult(correct.GetCorrectExamPaper(db,UserNumber,strStatus,StartTime,EndTime,KeyWord,PageIndex,PageSize));
        }

        /// <summary>
        /// 获取参考学员
        /// </summary>
        /// <param name="examid"></param>
        /// <returns></returns>
        [HttpGet("GetExamStudent")]
        public JsonResult GetExamStudent(long examid)
        {
            return new JsonResult(correct.GetExamStudent(db,examid));
        }

        /// <summary>
        /// 评分
        /// </summary>
        /// <param name="RecordID"></param>
        /// <param name="ItemID"></param>
        /// <param name="Score"></param>
        /// <returns></returns>
        [HttpPut("CorrectQuestion")]
        public async Task<JsonResult> CorrectQuestion(long RecordID, long ItemID, int Score)
        {
            return new JsonResult(await correct.CorrectQuestion(db,RecordID,ItemID,Score));
        }

        /// <summary>
        /// 提交结果
        /// </summary>
        /// <param name="ExamID"></param>
        /// <returns></returns>
        [HttpPut("CorrectSubmitTestPaper")]
        public JsonResult CorrectSubmitTestPaper(long ExamID)
        {
            var requestJWT = Request.Headers["Authorization"];
            TokenModel obj = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenModel>(client.GetTokenJson(requestJWT).Result);
            return new JsonResult(new CorrectExamPaper().CorrectSubmitTestPaper(db,ExamID,client, obj,rabbit,configuration));

        }
        /// <summary>
        /// 训练任务科目-提交考试结果
        /// </summary>
        /// <param name="RecordID">记录ID</param>
        /// <param name="TaskID">任务ID</param>
        /// <param name="SubjectID"></param>
        /// <param name="strResult"></param>
        /// <returns></returns>
        [HttpPut("CorrectTaskSubject")]
        public JsonResult CorrectTaskSubject(long RecordID, long TaskID, long SubjectID, string strResult, string subjectName)
        {
            var requestJWT = Request.Headers["Authorization"];
            TokenModel obj = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenModel>(client.GetTokenJson(requestJWT).Result);
            return new JsonResult(new CorrectExamPaper().CorrectTaskSubject(db,rabbit,RecordID, TaskID,SubjectID,strResult,subjectName, obj));
        }

        /// <summary>
        /// 考试训练任务结果提交
        /// </summary>
        /// <param name="RecordID"></param>
        /// <param name="UserNumber"></param>
        /// <param name="TaskComment"></param>
        /// <param name="Result"></param>
        /// <returns></returns>
        [HttpPut("CorrectTask")]
        public JsonResult CorrectTask(long RecordID, string UserNumber, string TaskComment, string Result, string taskName)
        {
            var requestJWT = Request.Headers["Authorization"];
            TokenModel obj = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenModel>(client.GetTokenJson(requestJWT).Result);
            return new JsonResult(new CorrectExamPaper().CorrectTask(db,rabbit,RecordID,UserNumber,TaskComment,Result,taskName, obj));
        }

        /// <summary>
        /// 获取实践考试作答界面
        /// </summary>
        /// <param name="examid"></param>
        /// <param name="UserNumber"></param>
        /// <returns></returns>
        [HttpGet("GetTaskSubjectInfo")]
        public JsonResult GetTaskSubjectInfo(long examid,string UserNumber)
        {
            return new JsonResult(correct.GetTaskSubjectInfo(db,examid, UserNumber));
        }
        /// <summary>
        /// 更新阅卷状态
        /// </summary>
        /// <param name="examid"></param>
        /// <param name="UserNumber"></param>
        /// <returns></returns>
        [HttpPut("UpdateStuCorrectStatus")]
        public JsonResult UpdateStuCorrectStatus(long examid, string UserNumber)
        {
            var requestJWT = Request.Headers["Authorization"];
            TokenModel obj = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenModel>(client.GetTokenJson(requestJWT).Result);
            return new JsonResult(new CorrectExamPaper().UpdateStuCorrectStatus(db,rabbit,examid,UserNumber, obj));
        }
    }
}