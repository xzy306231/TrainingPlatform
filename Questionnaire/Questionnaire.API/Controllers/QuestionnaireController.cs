using System.Collections.Generic;
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
    public class QuestionnaireController : ControllerBase
    {
        private readonly IHttpClientHelper client;
        private readonly QuestionnaireBLL questionnaire;
        private readonly pf_questionnaireContext db;
        private readonly RabbitMQClient rabbit;
        public QuestionnaireController(IHttpClientHelper client, QuestionnaireBLL questionnaire,pf_questionnaireContext db, RabbitMQClient rabbit)
        {
            this.client = client;
            this.questionnaire = questionnaire;
            this.rabbit = rabbit;
            this.db = db;
        }
        /// <summary>
        /// 获取所有问卷
        /// </summary>
        /// <param name="PlanID">计划ID</param>
        /// <param name="CourseID">课程ID</param>
        /// <param name="strQuestionTheme">问卷主题</param>
        /// <returns></returns>
        [HttpGet("GetQuestionnaire")]
        public JsonResult GetQuestionnaire(long PlanID, long CourseID, string strQuestionTheme)
        {
            return new JsonResult(questionnaire.GetQuestionnaire(db,PlanID,CourseID, strQuestionTheme));
        }

        /// <summary>
        /// 修改问卷名称
        /// </summary>
        /// <param name="ID">问卷ID</param>
        /// <param name="Name">问卷名称</param>
        /// <returns></returns>
        [HttpPut("Update_QuestionnaireName")]
        public JsonResult Update_QuestionnaireName(long ID, string Name)
        {
            var requestJWT = Request.Headers["Authorization"];
            TokenModel obj = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenModel>(client.GetTokenJson(requestJWT).Result);
            return new JsonResult(questionnaire.Update_QuestionnaireName(db,rabbit, ID,Name,obj));
        }

        /// <summary>
        /// 预览问卷
        /// </summary>
        /// <param name="QuestionnaireID">问卷ID</param>
        /// <returns></returns>
        [HttpGet("PreviewQuestionnaire")]
        public JsonResult PreviewQuestionnaire(long QuestionnaireID)
        {
            return new JsonResult(questionnaire.PreviewQuestionnaire(db,QuestionnaireID));
        }

        /// <summary>
        /// 获取问卷结果
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        [HttpGet("GetQuestionnaireResult")]
        public JsonResult GetQuestionnaireResult(long ID)
        {
            return new JsonResult(questionnaire.GetQuestionnaireResult(db,ID));
        }

        /// <summary>
        /// 创建问卷
        /// </summary>
        /// <param name="qt">问卷对象</param>
        /// <returns></returns>
        [HttpPost("CreateQuestionnaire")]
        public JsonResult CreateQuestionnaire(QuestionnaireInfo qt)
        {
            var requestJWT = Request.Headers["Authorization"];
            TokenModel obj = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenModel>(client.GetTokenJson(requestJWT).Result);
            return new JsonResult(questionnaire.CreateQuestionnaire(db,rabbit, qt,obj));
        }

        /// <summary>
        /// 创建题目
        /// </summary>
        /// <param name="QuestionnaireID">问卷ID</param>
        /// <param name="TitleInfo">题干</param>
        /// <param name="ItemType">试题类型</param>
        /// <param name="CreateBy">创建人</param>
        /// <returns></returns>
        [HttpPost("CreateQuestionnaireItem")]
        public JsonResult CreateQuestionnaireItem(long QuestionnaireID, string TitleInfo, string ItemType, long CreateBy)
        {
            var requestJWT = Request.Headers["Authorization"];
            TokenModel obj = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenModel>(client.GetTokenJson(requestJWT).Result);
            return new JsonResult(questionnaire.CreateQuestionnaireItem(db,rabbit,QuestionnaireID, TitleInfo, ItemType, CreateBy,obj));
        }

        /// <summary>
        /// 修改题目
        /// </summary>
        /// <param name="ID">题目ID</param>
        /// <param name="TitleInfo">题目信息</param>
        /// <param name="UpdateBy">修改人</param>
        /// <returns></returns>
        [HttpPut("UpdateQuestionnaireItem")]
        public JsonResult UpdateQuestionnaireItem(long ID, string TitleInfo, long UpdateBy)
        {
            var requestJWT = Request.Headers["Authorization"];
            TokenModel obj = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenModel>(client.GetTokenJson(requestJWT).Result);
            return new JsonResult(questionnaire.UpdateQuestionnaireItem(db,rabbit,ID, TitleInfo, UpdateBy,obj));
        }

        /// <summary>
        /// 删除题目
        /// </summary>
        /// <param name="ItemID">题目ID</param>
        /// <returns></returns>
        [HttpDelete("RemoveQuestionnaireItem")]
        public JsonResult RemoveQuestionnaireItem(long ItemID)
        {
            var requestJWT = Request.Headers["Authorization"];
            TokenModel obj = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenModel>(client.GetTokenJson(requestJWT).Result);
            return new JsonResult(questionnaire.RemoveQuestionnaireItem(db,rabbit,ItemID,obj));
        }

        /// <summary>
        /// 设置问题是否必答
        /// </summary>
        /// <param name="id">ID</param>
        /// <param name="result">结果</param>
        /// <returns></returns>
        [HttpPut("SetQuestionIsMustAnswer")]
        public JsonResult SetQuestionIsMustAnswer(long id, sbyte? result)
        {
            var requestJWT = Request.Headers["Authorization"];
            TokenModel obj = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenModel>(client.GetTokenJson(requestJWT).Result);
            return new JsonResult(questionnaire.SetQuestionIsMustAnswer(db,rabbit,id,result,obj));
        }

        /// <summary>
        /// 设置问题作答数量(最少)   
        /// </summary>
        /// <param name="id"></param>
        /// <param name="minCount"></param>
        /// <returns></returns>
        [HttpPut("SetAnswerQuestionCountMin")]
        public JsonResult SetAnswerQuestionCountMin(long id, int? minCount)
        {
            var requestJWT = Request.Headers["Authorization"];
            TokenModel obj = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenModel>(client.GetTokenJson(requestJWT).Result);
            return new JsonResult(questionnaire.SetAnswerQuestionCountMin(db,rabbit,id, minCount,obj));
        }

        /// <summary>
        /// 设置问题作答数量(最多)   
        /// </summary>
        /// <param name="id"></param>
        /// <param name="maxCount"></param>
        /// <returns></returns>
        [HttpPut("SetAnswerQuestionCountMax")]
        public JsonResult SetAnswerQuestionCountMax(long id, int? maxCount)
        {
            var requestJWT = Request.Headers["Authorization"];
            TokenModel obj = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenModel>(client.GetTokenJson(requestJWT).Result);
            return new JsonResult(questionnaire.SetAnswerQuestionCountMax(db,rabbit,id, maxCount,obj));
        }

        /// <summary>
        /// 问卷内容排序
        /// </summary>
        /// <param name="questionSort"></param>
        /// <returns></returns>
        [HttpPut("QuestionItemSort")]
        public JsonResult QuestionItemSort(List<QuestionSort> questionSort)
        {
            return new JsonResult(questionnaire.QuestionItemSort(db,questionSort));
        }

        /// <summary>
        /// 创建题目选项
        /// </summary>
        /// <param name="ItemID">题目ID</param>
        /// <param name="OptionContent">选项内容</param>
        /// <param name="CreateBy">创建人</param>
        /// <returns></returns>
        [HttpPost("CreateQuestionnaireItemOption")]
        public JsonResult CreateQuestionnaireItemOption(long ItemID,string OptionNum, string OptionContent, long CreateBy)
        {
            var requestJWT = Request.Headers["Authorization"];
            TokenModel obj = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenModel>(client.GetTokenJson(requestJWT).Result);
            return new JsonResult(questionnaire.CreateQuestionnaireItemOption(db,rabbit,ItemID,OptionNum, OptionContent, CreateBy,obj));
        }

        /// <summary>
        /// 更新选项字母
        /// </summary>
        /// <param name="optionContents"></param>
        /// <returns></returns>
        [HttpPut("UpdateItemOption")]
        public async Task<object> UpdateItemOption(List<OptionContent> optionContents)
        {
            return new JsonResult(await questionnaire.UpdateItemOption(db,optionContents));
        }
        /// <summary>
        /// 修改题目选项
        /// </summary>
        /// <param name="ID">选项ID</param>
        /// <param name="OptionContent">选项内容</param>
        /// <param name="UpdateBy">修改人</param>
        /// <returns></returns>
        [HttpPut("UpdateQuestionnaireItemOption")]
        public JsonResult UpdateQuestionnaireItemOption(long ID, string OptionContent, long UpdateBy)
        {
            var requestJWT = Request.Headers["Authorization"];
            TokenModel obj = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenModel>(client.GetTokenJson(requestJWT).Result);
            return new JsonResult(questionnaire.UpdateQuestionnaireItemOption(db,rabbit,ID, OptionContent, UpdateBy,obj));
        }

        /// <summary>
        /// 删除题目选项
        /// </summary>
        /// <param name="ItemOptionID">题目选项ID</param>
        /// <returns></returns>
        [HttpDelete("RemoveQuestionnaireItemOption")]
        public JsonResult RemoveQuestionnaireItemOption(long ItemOptionID)
        {
            var requestJWT = Request.Headers["Authorization"];
            TokenModel obj = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenModel>(client.GetTokenJson(requestJWT).Result);
            return new JsonResult(questionnaire.RemoveQuestionnaireItemOption(db,rabbit,ItemOptionID,obj));
        }
        /// <summary>
        /// 问卷发布
        /// </summary>
        /// <param name="QuestionnaireID">问卷ID</param>
        /// <returns></returns>

        [HttpPut("PublishQuestionnaire")]
        public JsonResult PublishQuestionnaire(long QuestionnaireID)
        {
            var requestJWT = Request.Headers["Authorization"];
            TokenModel obj = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenModel>(client.GetTokenJson(requestJWT).Result);
            return new JsonResult(questionnaire.PublishQuestionnaire(db,rabbit,QuestionnaireID,obj));
        }

        /// <summary>
        /// 终止问卷
        /// </summary>
        /// <param name="id">问卷ID</param>
        /// <returns></returns>
        [HttpPut("QuitQuestionnaire")]
        public JsonResult QuitQuestionnaire(long id)
        {
            var requestJWT = Request.Headers["Authorization"];
            TokenModel obj = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenModel>(client.GetTokenJson(requestJWT).Result);
            return new JsonResult(questionnaire.QuitQuestionnaire(db,rabbit,id,obj));
        }

        /// <summary>
        /// 根据培训计划ID，中止问卷
        /// </summary>
        /// <param name="PlanID"></param>
        /// <returns></returns>
        [HttpPost("QuitQuestionByPlanID")]
        public JsonResult QuitQuestionByPlanID(Plan PlanID)
        {
            var requestJWT = Request.Headers["Authorization"];
            TokenModel obj = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenModel>(client.GetTokenJson(requestJWT).Result);
            return new JsonResult(questionnaire.QuitQuestionByPlanID(db,rabbit,PlanID,obj));
        }

        [HttpPost("QuitQuestionByRemote")]
        public JsonResult QuitQuestionByRemote(Plan PlanID)
        {
            return new JsonResult(questionnaire.QuitQuestionByRemote(db,rabbit,PlanID));
        }

        /// <summary>
        /// 删除问卷
        /// </summary>
        /// <param name="QuestionnaireID">问卷ID</param>
        /// <returns></returns>
        [HttpDelete("DeleteQuestionnaire")]
        public JsonResult DeleteQuestionnaire(long QuestionnaireID)
        {
            var requestJWT = Request.Headers["Authorization"];
            TokenModel obj = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenModel>(client.GetTokenJson(requestJWT).Result);
            return new JsonResult(questionnaire.DeleteQuestionnaire(db,rabbit,QuestionnaireID,obj));
        }
    }
}