using Course.BLL;
using Microsoft.AspNetCore.Mvc;

namespace Course.API.Controllers
{
    /// <summary>
    /// 统计分析数据
    /// </summary>
    [Route("course/v1")]
    [ApiController]
    public class StatisticDataController: ControllerBase
    {
        #region 培训统计

        /// <summary>
        /// 获取当前正在进行的培训计划数量
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetCurrentTrainingPlanCount")]
        public JsonResult GetCurrentTrainingPlanCount()
        {
            return new JsonResult(new StatisticData().GetCurrentTrainingPlanCount());
        }

        /// <summary>
        /// 获取培训计划信息
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetTrainingPlanInfo")]
        public JsonResult GetTrainingPlanInfo()
        {
            return new JsonResult(new StatisticData().GetTrainingPlanInfo());
        }

        /// <summary>
        /// 按年份查找培训计划中人数
        /// </summary>
        /// <param name="year"></param>
        /// <returns></returns>
        [HttpGet("GetTrainingPlanInfoByYear")]
        public JsonResult GetTrainingPlanInfoByYear(int year)
        {
            return new JsonResult(new StatisticData().GetTrainingPlanInfoByYear(year));
        }

        /// <summary>
        ///  获取培训计划完成率
        /// </summary>
        /// <param name="IsAsc"></param>
        /// <param name="FeildName"></param>
        /// <returns></returns>
        [HttpGet("GetTrainingPlanFinishRate")]
        public JsonResult GetTrainingPlanFinishRate(bool IsAsc=false, string FeildName="")
        {
            return new JsonResult(new StatisticData().GetTrainingPlanFinishRate(IsAsc,FeildName));
        }

        #endregion

        #region 理论教学

        /// <summary>
        /// 获取培训计划平均学习时长
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetAvgLearningTime")]
        public JsonResult GetAvgLearningTime()
        {
            return new JsonResult(new StatisticData().GetAvgLearningTime());
        }

        /// <summary>
        /// 获取课程学习完成率
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetCourseFinishRate")]
        public JsonResult GetCourseFinishRate(bool IsAsc, string FeildName)
        {
            return new JsonResult(new StatisticData().GetCourseFinishRate(IsAsc,FeildName));
        }

        /// <summary>
        /// 获取知识点掌握度
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetKnowledgeDegree")]
        public JsonResult GetKnowledgeDegree()
        {
            return new JsonResult(new StatisticData().GetKnowledgeDegree());
        }

        #endregion

    }

}
