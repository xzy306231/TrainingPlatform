using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Examination.BLL;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Examination.API.Controllers
{
    [Route("examination/v1")]
    [ApiController]
    public class StatisticDataController : ControllerBase
    {
        private readonly StatisticData statisticData;
        private readonly pf_examinationContext db;
        public StatisticDataController(StatisticData statisticData, pf_examinationContext db)
        {
            this.statisticData = statisticData;
            this.db = db;
        }
        /// <summary>
        /// 考试提交率分析
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetExamSubmitRate")]
        public JsonResult GetExamSubmitRate()
        {
            return new JsonResult(statisticData.GetExamSubmitRate(db));
        }
        /// <summary>
        /// 科目分析
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetSubjectCorrectRate")]
        public JsonResult GetSubjectCorrectRate()
        {
            return new JsonResult(statisticData.GetSubjectCorrectRate(db));
        }

        /// <summary>
        /// 获取理论考试正确率排名
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetTheoryExamCorrectRateRank")]
        public JsonResult GetTheoryExamCorrectRateRank()
        {
            return new JsonResult(statisticData.GetTheoryExamCorrectRateRank(db));
        }

        /// <summary>
        /// 获取实践考试通过率排名
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetTaskPassRateRank")]
        public JsonResult GetTaskPassRateRank()
        {
            return new JsonResult(statisticData.GetTaskPassRateRank(db));
        }

        /// <summary>
        /// 获取理论考试知识点掌握度
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetExamKnowledge")]
        public JsonResult GetExamKnowledge()
        {
            return new JsonResult(statisticData.GetExamKnowledge(db));
        }
    }
}