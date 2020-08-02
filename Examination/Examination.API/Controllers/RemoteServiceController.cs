using System.Collections.Generic;
using Examination.BLL;
using Microsoft.AspNetCore.Mvc;

namespace Examination.API.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route("examination/v1")]
    [ApiController]
    public class RemoteServiceController : ControllerBase
    {
        private readonly RemoteService remoteService;
        private readonly IHttpClientHelper client;
        private readonly pf_examinationContext db;
        private readonly RabbitMQClient rabbit;
        public RemoteServiceController(IHttpClientHelper client, RemoteService remoteService, pf_examinationContext db,RabbitMQClient rabbit)
        {
            this.client = client;
            this.remoteService = remoteService;
            this.db = db;
            this.rabbit = rabbit;
        }

        /// <summary>
        /// 从远程服务添加学员
        /// </summary>
        /// <param name="examUserModel"></param>
        /// <returns></returns>
        [HttpPost("AddStuFromRemoteService")]
        public JsonResult AddStuFromRemoteService(ExamUserModel examUserModel)
        {
            return new JsonResult(remoteService.AddStuFromRemoteService(db,rabbit,examUserModel));
        }
       /// <summary>
       /// 
       /// </summary>
       /// <param name="examUserModel"></param>
       /// <returns></returns>
        [HttpPut("DeleteStuFromRemoteService")]
        public JsonResult DeleteStuFromRemoteService(ExamUserModel examUserModel)
        {
            return new JsonResult(remoteService.DeleteStuFromRemoteService(db,rabbit,examUserModel));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="examUserModel"></param>
        /// <returns></returns>
        [HttpPut("DeleteStuPlanIDExamID")]
        public JsonResult DeleteStuPlanIDExamID(ExamUserModel examUserModel)
        {
            return new JsonResult(remoteService.DeleteStuPlanIDExamID(db,examUserModel));
        }

        /// <summary>
        /// 恢复考试管理
        /// </summary>
        /// <param name="examUserModel"></param>
        /// <returns></returns>
        [HttpPut("RecoverExamination")]
        public JsonResult RecoverExamination(ExamUserModel examUserModel)
        {
            return new JsonResult(remoteService.RecoverExamination(db,examUserModel));
        }

        /// <summary>
        /// 发布考试
        /// </summary>
        /// <param name="examination"></param>
        /// <returns></returns>
        [HttpPut("PublishExamination")]
        public JsonResult PublishExamination(List<ExaminationInfo> examination)
        {
            return new JsonResult(remoteService.PublishExamination(db,rabbit,examination));
        }
        /// <summary>
        /// 更新考试的起始与结束时间
        /// </summary>
        /// <param name="examinations"></param>
        /// <returns></returns>
        [HttpPut("UpdateExaminationTime")]
        public JsonResult UpdateExaminationTime(List<ExaminationInfo> examinations)
        {
            return new JsonResult(remoteService.UpdateExaminationTime(db,examinations));
        }

        /// <summary>
        /// 强制提交试卷
        /// </summary>
        /// <param name="examId"></param>
        /// <returns></returns>
        [HttpPut("ForceSubmitExamination")]
        public JsonResult ForceSubmitExamination(long examId)
        {
            return new JsonResult(remoteService.ForceSubmitExamination(db,examId));
        }

        /// <summary>
        /// 查看考试是否通过
        /// </summary>
        /// <param name="examId"></param>
        /// <param name="userNumber"></param>
        /// <returns></returns>
        [HttpGet("GetStuExamResult")]
        public JsonResult GetStuExamResult(long examId, string userNumber)
        {
            return new JsonResult(remoteService.GetStuExamResult(db,examId,userNumber));
        }
        /// <summary>
        /// 提前中止考试
        /// </summary>
        /// <param name="examUserModel"></param>
        /// <returns></returns>
        [HttpPut("QuitExamination")]
        public object QuitExamination(ExamUserModel examUserModel)
        {
            return new JsonResult(remoteService.QuitExamination(db,rabbit,examUserModel));
        }

        /// <summary>
        /// 获取培训计划下的考试进度
        /// </summary>
        /// <param name="planId"></param>
        /// <returns></returns>
        [HttpGet("GetPlanExamProgress")]
        public JsonResult GetPlanExamProgress(long planId)
        {
            return new JsonResult(remoteService.GetPlanExamProgress(db,planId));
        }

        /// <summary>
        /// 查找评分教员
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        [HttpPost("GetExamGradeTech")]
        public JsonResult GetExamGradeTech(List<long> list)
        {
            return new JsonResult(remoteService.GetExamGradeTech(db,list));
        }

        [HttpPost("GetExamGradeTechNum")]
        public object GetExamGradeTechNum(List<long> examIdList)
        {
            return new JsonResult(remoteService.GetExamGradeTechNum(db,examIdList));
        }
    }
}