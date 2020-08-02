using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PaperQuestions.BLL;

namespace PaperQuestions.API.Controllers
{
    /// <summary>
    /// 试卷审核
    /// </summary>
    [Route("paperquestion/v1")]
    [ApiController]
    public class ApprovalPapersController : ControllerBase
    {
        private readonly IHttpClientHelper _client;
        private readonly ExaminationPapers _papers;
        private readonly pf_exam_paper_questionsContext db;
        private readonly RabbitMQClient rabbit;
        public ApprovalPapersController(IHttpClientHelper client, ExaminationPapers papers, pf_exam_paper_questionsContext db,RabbitMQClient rabbit)
        {
            _client = client;
            this._papers = papers;
            this.db = db;
            this.rabbit = rabbit;
        }

        #region 试卷审核

        /// <summary>
        /// 未审核
        /// </summary>
        /// <param name="keyWord"></param>
        /// <param name="IsAsc"></param>
        /// <param name="FieldName"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet("GetNotApprovalTestPapers")]
        public JsonResult GetNotApprovalTestPapers(string keyWord, bool IsAsc, string FieldName, int pageIndex = 1, int pageSize = 10)
        {
            return new JsonResult(_papers.GetNotApprovalTestPapers(db,keyWord, IsAsc, FieldName, pageIndex, pageSize));
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
        [HttpGet("GetApprovaledTestPapers")]
        public JsonResult GetApprovaledTestPapers(string keyWord, bool IsAsc, string FieldName, int pageIndex = 1, int pageSize = 10)
        {
            return new JsonResult(_papers.GetApprovaledTestPapers(db,keyWord, IsAsc, FieldName, pageIndex, pageSize));
        }

        [HttpGet("GetPassedTestPapers")]
        public JsonResult GetPassedTestPapers(string keyWord, bool IsAsc, string FieldName, int pageIndex = 1, int pageSize = 10)
        {
            return new JsonResult(_papers.GetPassedTestPapers(db,keyWord, IsAsc, FieldName, pageIndex, pageSize));
        }
        /// <summary>
        /// 审核
        /// </summary>
        /// <param name="paperQuestionApprovals"></param>
        /// <returns></returns>
        [HttpPut("ApprovalTestPapers")]
        public async Task<JsonResult> ApprovalTestPapers(List<PaperQuestionApproval> paperQuestionApprovals)
        {
            var requestJWT = Request.Headers["Authorization"];
            TokenModel obj = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenModel>(_client.GetTokenJson(requestJWT).Result);
            return new JsonResult(await _papers.ApprovalTestPapers(db,rabbit,paperQuestionApprovals,obj));
        }
        /// <summary>
        /// 试卷信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("GetTestPaperInfo")]
        public object GetTestPaperInfo(long id)
        {
            return new JsonResult(_papers.GetTestPaperInfo(db,id));
        }

        #endregion
    }
}
