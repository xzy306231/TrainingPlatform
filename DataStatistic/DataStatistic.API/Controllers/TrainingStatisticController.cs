using DataStatistic.BLL;
using Microsoft.AspNetCore.Mvc;

namespace DataStatistic.API.Controllers
{
    /// <summary>
    /// 系统总业务统计数据
    /// </summary>
    [Route("statistic/v1")]
    [ApiController]
    public class TrainingStatisticController : ControllerBase
    {
        private readonly TrainingStatistic statistic;
        private readonly pf_datastatisticContext db;
        public TrainingStatisticController(TrainingStatistic statistic, pf_datastatisticContext db)
        {
            this.statistic = statistic;
            this.db = db;
        }
        /// <summary>
        /// 获取平台统计数据
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetPFStatisticData")]
        public JsonResult GetPFStatisticData()
        {
            return new JsonResult(statistic.GetPFStatisticData(db));
        }

        /// <summary>
        /// 获取每年中每月的培训次数
        /// </summary>
        /// <param name="year"></param>
        /// <returns></returns>
        [HttpGet("GetTrainingTimesByYear")]
        public JsonResult GetTrainingTimesByYear(string year)
        {
            return new JsonResult(statistic.GetTrainingTimesByYear(db,year));
        }
        /// <summary>
        /// 获取每年的培训次数
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetTrainingYearTimes")]
        public JsonResult GetTrainingYearTimes()
        {
            return new JsonResult(statistic.GetTrainingYearTimes(db));
        }
    }
}