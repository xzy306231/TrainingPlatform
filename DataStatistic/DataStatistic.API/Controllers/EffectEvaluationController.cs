using System.Collections.Generic;
using DataStatistic.BLL;
using Microsoft.AspNetCore.Mvc;

namespace DataStatistic.API.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route("statistic/v1")]
    [ApiController]
    public class EffectEvaluationController : ControllerBase
    {
        private readonly EffectEvaluation evaluation;
        private readonly pf_datastatisticContext db;
        public EffectEvaluationController(EffectEvaluation evaluation, pf_datastatisticContext db)
        {
            this.evaluation = evaluation;
            this.db = db;
        }
        /// <summary>
        /// 获取指标内容
        /// </summary>
        /// <param name="planid"></param>
        /// <returns></returns>
        [HttpGet("GetIndexContent")]
        public JsonResult GetIndexContent(long planid)
        {
            return new JsonResult(evaluation.GetIndexContent(db,planid));
        }
        /// <summary>
        /// 指标状态变更
        /// </summary>
        /// <param name="indexContents"></param>
        /// <returns></returns>
        [HttpPost("SaveIndexStatus")]
        public JsonResult SaveIndexStatus(List<IndexContent> indexContents)
        {
            return new JsonResult(evaluation.SaveIndexStatus(db,indexContents));
        }

        /// <summary>
        ///  获取结束语
        /// </summary>
        /// <param name="level"></param>
        /// <param name="div"></param>
        /// <returns></returns>
        [HttpGet("GetEndCommment")]
        public JsonResult GetEndCommment(string level,string div)
        {
            return new JsonResult(evaluation.GetEndCommment(db,level,div));
        }
    }
}