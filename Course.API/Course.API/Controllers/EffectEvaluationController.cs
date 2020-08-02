using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Course.BLL;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Course.API.Controllers
{
    [Route("course/v1")]
    [ApiController]
    public class EffectEvaluationController : ControllerBase
    {
        private IHttpClientHelper client;
        public EffectEvaluationController(IHttpClientHelper client)
        {
            this.client = client;
        }
        /// <summary>
        /// 获取已结束的培训计划
        /// </summary>
        /// <param name="keyWord"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet("GetEndTrainingPlan")]
        public JsonResult GetEndTrainingPlan(string keyWord, string startTime, string endTime, int pageIndex=1, int pageSize=10)
        {
            return new JsonResult(new EffectEvaluation().GetEndTrainingPlan(keyWord,startTime,endTime,pageIndex,pageSize));
        }
        /// <summary>
        /// 获取个人报告培训计划信息
        /// </summary>
        /// <param name="userNumber"></param>
        /// <param name="planId"></param>
        /// <returns></returns>
        [HttpGet("GetPersonalPlanInfo")]
        public JsonResult GetPersonalPlanInfo(string userNumber, long planId)
        {
            return new JsonResult(new EffectEvaluation().GetPersonalPlanInfo(userNumber,planId));
        }
        /// <summary>
        /// 获取培训计划下的学员
        /// </summary>
        /// <param name="planId"></param>
        /// <returns></returns>
        [HttpGet("GetStuByPlanID")]
        public JsonResult GetStuByPlanID(long planId)
        {
            return new JsonResult(new EffectEvaluation().GetStuByPlanID(planId));
        }
        /// <summary>
        /// 获取个人理论部分
        /// </summary>
        /// <param name="userNumber"></param>
        /// <param name="planId"></param>
        /// <returns></returns>
        [HttpGet("GetPersonalTheoryReportResult")]
        public JsonResult GetPersonalTheoryReportResult(string userNumber, long planId)
        {
            return new JsonResult(new EffectEvaluation().GetPersonalTheoryReportResult(userNumber,planId));
        }

        #region 整体报告
        /// <summary>
        /// 培训计划基本信息
        /// </summary>
        /// <param name="planId"></param>
        /// <returns></returns>
        [HttpGet("GetPlanReportResult")]
        public JsonResult GetPlanReportResult(long planId)
        {
            return new JsonResult(new EffectEvaluation().GetPlanReportResult(planId));
        }
        /// <summary>
        /// 理论学习部分效果评估
        /// </summary>
        /// <param name="planId"></param>
        /// <param name="client"></param>
        /// <returns></returns>
        [HttpGet("GetPlanTheoryReportResult")]
        public JsonResult GetPlanTheoryReportResult(long planId)
        {
            return new JsonResult(new EffectEvaluation().GetPlanTheoryReportResult(planId));
        }

        #endregion

        #region 报告生成

        /// <summary>
        /// 获取个人报告
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="userNumber"></param>
        /// <param name="planId"></param>
        /// <returns></returns>
        [HttpGet("GetPersonnalReport")]
        public JsonResult GetPersonnalReport(long userId, string userNumber, long planId)
        {
            return new JsonResult(new EffectEvaluation().GetPersonnalReport(userId,userNumber, planId,client));
        }

        /// <summary>
        /// 获取计划的报告
        /// </summary>
        /// <param name="planId"></param>
        /// <returns></returns>
        [HttpGet("GetPlanReport")]
        public JsonResult GetPlanReport(long planId)
        {
            return new JsonResult(new EffectEvaluation().GetPlanReport(planId,client));
        }

        #endregion
    }
}