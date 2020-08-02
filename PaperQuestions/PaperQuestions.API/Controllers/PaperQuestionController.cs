using Microsoft.AspNetCore.Mvc;
using PaperQuestions.BLL;

namespace PaperQuestions.API.Controllers
{
    /// <summary>
    /// 试卷导入
    /// </summary>
    [Route("paperquestion/v1")]
    [ApiController]
    public class PaperQuestionController : ControllerBase
    {
        private readonly PaperQuestion paperQuestion;
        private readonly pf_exam_paper_questionsContext db;
        private readonly RabbitMQClient rabbit;
        public PaperQuestionController(PaperQuestion paperQuestion, pf_exam_paper_questionsContext db,RabbitMQClient rabbit)
        {
            this.paperQuestion = paperQuestion;
            this.db = db;
            this.rabbit = rabbit;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="paperInfomation"></param>
        /// <returns></returns>
        [HttpPost("TestPaperImportToDB")]
        public JsonResult TestPaperImportToDB(PaperInfomation paperInfomation)
        {
            return new JsonResult(paperQuestion.TestPaperImportToDB(db,rabbit,paperInfomation));
        }
    }
}