using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataStatistic.BLL;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DataStatistic.API.Controllers
{
    /// <summary>
    /// 模拟训练
    /// </summary>
    [Route("statistic/v1")]
    [ApiController]
    public class TaskTrainingController : ControllerBase
    {
        private readonly TaskTraining taskTraining;
        private readonly pf_datastatisticContext db;
        public TaskTrainingController(TaskTraining taskTraining, pf_datastatisticContext db)
        {
            this.taskTraining = taskTraining;
            this.db = db;
        }
        /// <summary>
        /// 获取任务通过率排名
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetTaskRank")]
        public JsonResult GetTaskRank()
        {
            return new JsonResult(taskTraining.GetTaskRank(db));
        }
        /// <summary>
        /// 获取任务的知识点掌握度
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetTaskKnowledge")]
        public JsonResult GetTaskKnowledge()
        {
            return new JsonResult(taskTraining.GetTaskKnowledge(db));
        }
    }
}