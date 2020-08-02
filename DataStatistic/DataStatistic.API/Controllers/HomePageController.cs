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
    /// 个人中心
    /// </summary>
    [Route("statistic/v1")]
    [ApiController]
    public class HomePageController : ControllerBase
    {
        private readonly HomePage homePage;
        private readonly pf_datastatisticContext db;
        public HomePageController(HomePage homePage, pf_datastatisticContext db)
        {
            this.homePage = homePage;
            this.db = db;
        }
        /// <summary>
        /// 个人中心主页界面数据查询
        /// </summary>
        /// <param name="userNumber"></param>
        /// <returns></returns>
        [HttpGet("GetStuStatisticData")]
        public JsonResult GetStuStatisticData(string userNumber)
        {
            return new JsonResult(homePage.GetStuStatisticData(db,userNumber));
        }
    }
}