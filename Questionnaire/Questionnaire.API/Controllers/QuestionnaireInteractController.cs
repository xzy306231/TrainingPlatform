using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Questionnaire.BLL;

namespace Questionnaire.API.Controllers
{
    /// <summary>
    /// 教学互动
    /// </summary>
    [Route("questionnaire/v1")]
    [ApiController]
    public class QuestionnaireInteractController : ControllerBase
    {
        private readonly QuestionnaireInteract questionnaireInteract;
        private readonly pf_questionnaireContext db;
        private readonly RabbitMQClient rabbit;
        public QuestionnaireInteractController(QuestionnaireInteract questionnaireInteract,pf_questionnaireContext db,RabbitMQClient rabbit)
        {
            this.questionnaireInteract = questionnaireInteract;
            this.rabbit = rabbit;
            this.db = db;
        }
        /// <summary>
        /// 获取教学互动状态信息
        /// </summary>
        /// <param name="PlanID">计划ID</param>
        /// <param name="CourseID">课程ID</param>
        /// <param name="strStatus">状态</param>
        /// <param name="keyWord">状态</param>
        /// <param name="UserID">用户ID</param>
        /// <param name="PageIndex">页码</param>
        /// <param name="PageSize">记录数</param>
        /// <returns></returns>
        [HttpGet("GetQuestionnaireByCourseID")]
        public JsonResult GetQuestionnaireByCourseID(long PlanID,long CourseID, string strStatus, string keyWord,long UserID, int PageIndex=1, int PageSize=10)
        {
            return new JsonResult(questionnaireInteract.GetQuestionnaireByCourseID(db,PlanID,CourseID,strStatus,keyWord,UserID,PageIndex,PageSize));
        }

        /// <summary>
        /// 问卷作答
        /// </summary>
        /// <param name="ModelList"></param>
        /// <returns></returns>
        [HttpPost("QuestionnaireInteractLog")]
        public async Task<JsonResult> QuestionnaireInteractLog(InteractionLogList ModelList)
        {
            return new JsonResult(new {code=200,result=await questionnaireInteract.QuestionnaireInteractLog(db,ModelList),msg="OK" });
        }

        /// <summary>
        /// 查看学员作答结果
        /// </summary>
        /// <param name="QuestionnaireID"></param>
        /// <param name="UserID"></param>
        /// <returns></returns>
        [HttpGet("GetQuestionnaireAnswerResult")]
        public JsonResult GetQuestionnaireAnswerResult(long QuestionnaireID, long UserID)
        {
            return new JsonResult(questionnaireInteract.GetQuestionnaireAnswerResult(db,QuestionnaireID, UserID));
        }
    }
}