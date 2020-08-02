using Examination.BLL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Examination.API.Controllers
{
    [Route("examination/v1")]
    [ApiController]
    public class ExaminationManageController : ControllerBase
    {
        private readonly IHttpClientHelper client;
        private readonly ExaminationManage examination;
        private readonly IConfiguration configuration;
        private readonly pf_examinationContext db;
        private readonly RabbitMQClient rabbit;
        public ExaminationManageController(IHttpClientHelper client, ExaminationManage examination, IConfiguration configuration, pf_examinationContext db,RabbitMQClient rabbit)
        {
            this.client = client;
            this.examination = examination;
            this.configuration = configuration;
            this.db = db;
            this.rabbit = rabbit;
        }

        /// <summary>
        /// 获取考试安排数据
        /// </summary>
        /// <param name="strStatus">状态</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="strKeyWord">关键字</param>
        /// <param name="PageIndex">页码</param>
        /// <param name="PageSize">记录数</param>
        /// <returns></returns>
        [HttpGet("GetExamination")]
        public JsonResult GetExamination(string strStatus, string startTime, string endTime, string strKeyWord, int PageIndex = 1, int PageSize = 10)
        {
            return new JsonResult(examination.GetExamination(db,strStatus, startTime, endTime, strKeyWord, PageIndex, PageSize));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strStatus"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="strKeyWord"></param>
        /// <param name="PageIndex"></param>
        /// <param name="PageSize"></param>
        /// <returns></returns>
        [HttpGet("GetExaminationStatistic")]
        public JsonResult GetExaminationStatistic(string strStatus, string startTime, string endTime, string strKeyWord, int PageIndex = 1, int PageSize = 10)
        {
            return new JsonResult(examination.GetExaminationStatistic(db,strStatus, startTime, endTime, strKeyWord, PageIndex, PageSize));
        }

        /// <summary>
        /// 将考试添加到培训计划
        /// </summary>
        /// <param name="KeyWord">搜索关键字</param>
        /// <param name="FieldName">排序字段</param>
        /// <param name="IsAsc">升序</param>
        /// <param name="PageIndex">页码</param>
        /// <param name="PageSize">记录数</param>
        /// <returns></returns>
        [HttpGet("GetExaminationToPlan")]
        public JsonResult GetExaminationToPlan(string KeyWord, string FieldName, bool IsAsc = false, int PageIndex = 1, int PageSize = 10)
        {
            return new JsonResult(examination.GetExaminationToPlan(db,KeyWord, FieldName, IsAsc, PageIndex, PageSize));
        }

        /// <summary>
        ///  更新考试复用状态
        /// </summary>
        /// <param name="examinations"></param>
        /// <param name="StartTime"></param>
        /// <param name="EndTime"></param>
        /// <returns></returns>
        [HttpPut("UpdateExaminationStatus")]
        public JsonResult UpdateExaminationStatus(List<ExaminationID> examinations)
        {
            var requestJWT = Request.Headers["Authorization"];
            TokenModel obj = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenModel>(client.GetTokenJson(requestJWT).Result);
            return new JsonResult(new ExaminationManage().UpdateExaminationStatus(db,rabbit,examinations, obj));
        }

        /// <summary>
        /// 查看学生考试作答状态
        /// </summary>
        /// <param name="examId"></param>
        /// <param name="userNumber"></param>
        /// <returns></returns>
        [HttpGet("GetStuExamStatus")]
        public JsonResult GetStuExamStatus(long examId, string userNumber)
        {
            return new JsonResult(examination.GetStuExamStatus(db,examId, userNumber));
        }

        /// <summary>
        /// 获取理论考试监控信息
        /// </summary>
        /// <param name="examinationid"></param>
        /// <param name="strExamStatus"></param>
        /// <param name="FieldName"></param>
        /// <param name="IsAsc"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet("GetTheoryExaminationStatus")]
        public JsonResult GetTheoryExaminationStatus(long examinationid, string strExamStatus, string FieldName, bool IsAsc = true, int pageIndex = 1, int pageSize = 5)
        {
            string str = configuration["DicUrl"];
            //部门
            string strDepartment = client.GetRemoteJson(str + "department_type").Result;
            DicModel dicDepartment = Newtonsoft.Json.JsonConvert.DeserializeObject<DicModel>(strDepartment);

            return new JsonResult(examination.GetTheoryExaminationStatus(db,examinationid, strExamStatus, IsAsc, FieldName, pageIndex, pageSize, dicDepartment));
        }

        /// <summary>
        /// 根据ID获取理论考试信息
        /// </summary>
        /// <param name="examid"></param>
        /// <returns></returns>
        [HttpGet("GetTheoryExaminationByID")]
        public JsonResult GetTheoryExaminationByID(long examid)
        {
            return new JsonResult(examination.GetTheoryExaminationByID(db,examid));
        }

        /// <summary>
        /// 创建理论考试安排
        /// </summary>
        /// <param name="examination"></param>
        /// <returns></returns>
        [HttpPost("Add_TheoryExamination")]
        public JsonResult Add_TheoryExamination(Examinations examination1)
        {
            var requestJWT = Request.Headers["Authorization"];
            TokenModel obj = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenModel>(client.GetTokenJson(requestJWT).Result);
            return new JsonResult(examination.Add_TheoryExamination(db,rabbit,examination1, client, obj));
        }

        /// <summary>        
        /// 修改理论考试
        /// </summary>
        /// <param name="examination"></param>
        /// <returns></returns>
        [HttpPut("Update_TheoryExamination")]
        public JsonResult Update_TheoryExamination(Examinations examination1)
        {
            var requestJWT = Request.Headers["Authorization"];
            TokenModel obj = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenModel>(client.GetTokenJson(requestJWT).Result);
            return new JsonResult(new ExaminationManage().Update_TheoryExamination(db,rabbit,examination1, obj));
        }

        [HttpGet("GetExamPaper")]
        public JsonResult GetExamPaper(long ExamID)
        {
            return new JsonResult(examination.GetExamPaper(db,ExamID));
        }
        /// <summary>
        /// 获取实践考试监控信息
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="strExamStatus"></param>
        /// <param name="FieldName"></param>
        /// <param name="IsAsc"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet("GetTaskExaminationStatus")]
        public JsonResult GetTaskExaminationStatus(long ID, string strExamStatus, string FieldName, bool IsAsc = true, int pageIndex = 1, int pageSize = 10)
        {
            string str = configuration["DicUrl"];
            //部门
            string strDepartment = client.GetRemoteJson(str + "department_type").Result;
            DicModel dicDepartment = Newtonsoft.Json.JsonConvert.DeserializeObject<DicModel>(strDepartment);
            return new JsonResult(examination.GetTaskExaminationStatus(db,ID, strExamStatus, IsAsc, FieldName, pageIndex, pageSize, dicDepartment));
        }

        /// <summary>
        /// 根据ID获取考试管理-实践考试信息
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        [HttpGet("GetTaskExaminationByID")]
        public JsonResult GetTaskExaminationByID(long ID)
        {
            return new JsonResult(examination.GetTaskExaminationByID(db,ID));
        }

        /// <summary>
        /// 添加实践考试
        /// </summary>
        /// <param name="taskExamination"></param>
        /// <returns></returns>
        [HttpPost("Add_TaskExamination")]
        public JsonResult Add_TaskExamination(TaskExamination taskExamination)
        {
            var requestJWT = Request.Headers["Authorization"];
            string url = client.GetTokenJson(requestJWT).Result;
            TokenModel obj = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenModel>(url);
            return new JsonResult(examination.Add_TaskExamination(db,rabbit,taskExamination, obj));
        }

        /// <summary>
        /// 修改实践考试
        /// </summary>
        /// <param name="taskExamination"></param>
        /// <returns></returns>
        [HttpPut("Update_TaskExamination")]
        public JsonResult Update_TaskExamination(TaskExamination taskExamination)
        {
            var requestJWT = Request.Headers["Authorization"];
            TokenModel obj = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenModel>(client.GetTokenJson(requestJWT).Result);
            return new JsonResult(examination.Update_TaskExamination(db,rabbit,taskExamination, obj));
        }

        /// <summary>
        /// 中止考试
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("Quit_Examination")]
        public JsonResult Quit_Examination(long id)
        {
            var requestJWT = Request.Headers["Authorization"];
            TokenModel obj = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenModel>(client.GetTokenJson(requestJWT).Result);
            return new JsonResult(examination.Quit_Examination(db,rabbit,id, obj));
        }

        /// <summary>
        /// 删除考试安排
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("Delete_Examination")]
        public JsonResult Delete_Examination(long id)
        {
            var requestJWT = Request.Headers["Authorization"];
            TokenModel obj = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenModel>(client.GetTokenJson(requestJWT).Result);
            return new JsonResult(examination.Delete_Examination(db,rabbit,id, obj));
        }

        [HttpGet("GetNotApprovalTestPapers")]
        public JsonResult GetNotApprovalTestPapers(string keyWord, bool IsAsc, string FieldName, int pageIndex = 1, int pageSize = 10)
        {
            return new JsonResult(examination.GetNotApprovalTestPapers(db,keyWord, IsAsc, FieldName, pageIndex, pageSize));
        }

        [HttpGet("GetApprovaledTestPapers")]
        public object GetApprovaledTestPapers(string keyWord, bool IsAsc, string FieldName, int pageIndex = 1, int pageSize = 10)
        {
            return new JsonResult(examination.GetApprovaledTestPapers(db,keyWord, IsAsc, FieldName, pageIndex, pageSize));
        }

        [HttpPut("ApprovalTestPapers")]
        public async Task<JsonResult> ApprovalTestPapers(List<PaperQuestionApproval> paperQuestionApprovals)
        {
            return new JsonResult(await examination.ApprovalTestPapers(db,paperQuestionApprovals));
        }

        [HttpPost("AddTheoryExamFromTestPapers")]
        public JsonResult AddTheoryExamFromTestPapers(Examinations examination1)
        {
            var requestJWT = Request.Headers["Authorization"];
            TokenModel obj = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenModel>(client.GetTokenJson(requestJWT).Result);
            return new JsonResult(examination.AddTheoryExamFromTestPapers(db,examination1, obj));
        }
        [HttpGet("GetTestPaperInfo")]
        public object GetTestPaperInfo(long id)
        {
            return new JsonResult(new ExaminationManage().GetTestPaperInfo(db,id));
        }
    }
}