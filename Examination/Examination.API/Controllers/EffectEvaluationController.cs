using Examination.BLL;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Examination.API.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route("examination/v1")]
    [ApiController]
    public class EffectEvaluationController: ControllerBase
    {
        private readonly EffectEvaluation evaluation;
        private readonly pf_examinationContext db;
        public EffectEvaluationController(EffectEvaluation evaluation, pf_examinationContext db)
        {
            this.evaluation = evaluation;
            this.db = db;
        }
        /// <summary>
        /// 个人考试报告
        /// </summary>
        /// <param name="userNumber"></param>
        /// <param name="planId"></param>
        /// <returns></returns>
        [HttpGet("GetPersonalExamReportResult")]
        public JsonResult GetPersonalExamReportResult(string userNumber, long planId)
        {
            return new JsonResult(evaluation.GetPersonalExamReportResult(db,userNumber, planId));
        }
        /// <summary>
        /// 培训计划考试报告
        /// </summary>
        /// <param name="planId"></param>
        /// <returns></returns>
        [HttpGet("GetPlanExamReportResult")]
        public JsonResult GetPlanExamReportResult(long planId)
        {
            return new JsonResult(evaluation.GetPlanExamReportResult(db,planId));
        }
    }
}
