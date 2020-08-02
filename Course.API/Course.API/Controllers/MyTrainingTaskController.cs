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
    public class MyTrainingTaskController : ControllerBase
    {
        /// <summary>
        /// 我的训练下拉列表
        /// </summary>
        /// <param name="UserNumber">用户名</param>
        /// <returns></returns>
        [HttpGet("GetMyTrainTaskPlanList")]
        public JsonResult GetMyTrainTaskPlanList(string UserNumber)
        {
            return new JsonResult(new MyTrainingTask().GetMyTrainTaskPlanList(UserNumber));
        }
        /// <summary>
        /// 获取我的训练数据
        /// </summary>
        /// <param name="PlanID">计划ID</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="strKeyWord">搜索关键字</param>
        /// <param name="UserID">用户名</param>
        /// <param name="pageSize">记录数</param>
        /// <param name="pageIndex">页码</param>
        /// <returns></returns>
        [HttpGet("GetMyTrainingTask")]
        public JsonResult GetMyTrainingTask(long PlanID, string startTime, string endTime, string strKeyWord, string UserID, int pageSize, int pageIndex)
        {
            return new JsonResult(new MyTrainingTask().GetMyTrainingTask(PlanID,startTime,endTime,strKeyWord,UserID,pageSize,pageIndex));
        }
    }
}