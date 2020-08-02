using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Course.API.Controllers
{
    /// <summary>
    /// 远程服务
    /// </summary>
    [Route("course/v1")]
    [ApiController]
    public class RemoteServiceController : ControllerBase
    {
        /// <summary>
        /// 更新学生考试得分
        /// </summary>
        /// <param name="examResults"></param>
        /// <returns></returns>
        [HttpPut("UpdateStuScore")]
        public JsonResult UpdateStuScore(List<ExamResult> examResults)
        {
            return new JsonResult(new RemoteService().UpdateStuScore(examResults));
        }

        /// <summary>
        ///  更新培训计划进度
        /// </summary>
        /// <param name="planProgresses"></param>
        /// <returns></returns>
        [HttpPut("UpdatePlanProgress")]
        public JsonResult UpdatePlanProgress(List<PlanProgress> planProgresses)
        {
            return new JsonResult(new RemoteService().UpdatePlanProgress(planProgresses));
        }

        /// <summary>
        /// 更新考试完成率
        /// </summary>
        /// <param name="examFinishRate"></param>
        /// <returns></returns>
        [HttpPut("UpdateExamFinishRate")]
        public JsonResult UpdateExamFinishRate(ExamFinishRate examFinishRate)
        {
            return new JsonResult(new RemoteService().UpdateExamFinishRate(examFinishRate));
        }
    }
}