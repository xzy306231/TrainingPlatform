using DataStatistic.BLL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace DataStatistic.API.Controllers
{
    /// <summary>
    /// 理论教学
    /// </summary>
    [Route("statistic/v1")]
    [ApiController]
    public class TheoryTeachingController : ControllerBase
    {
        private readonly IHttpClientHelper client;
        private readonly TheoryTeaching theoryTeaching;
        private readonly IConfiguration configuration;
        private readonly pf_datastatisticContext db;
        public TheoryTeachingController(IHttpClientHelper client, TheoryTeaching theoryTeaching,IConfiguration configuration, pf_datastatisticContext db)
        {
            this.client = client;
            this.theoryTeaching = theoryTeaching;
            this.configuration = configuration;
            this.db = db;
        }
        /// <summary>
        /// 获取学习时长排名
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetTheoryLearningTimeRank")]
        public JsonResult GetLearningTimeRank()
        {
            string str = configuration["DicUrl"];
            //部门
            string strDepartment = client.GetRequest(str + "department_type").Result;
            DicModel dicDepartment = Newtonsoft.Json.JsonConvert.DeserializeObject<DicModel>(strDepartment);

            return new JsonResult(theoryTeaching.GetLearningTimeRank(db,dicDepartment));
        }

        /// <summary>
        /// 获取理论知识点掌握度
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetTheoryKnowledge")]
        public JsonResult GetTheoryKnowledge()
        {
            return new JsonResult(theoryTeaching.GetTheoryKnowledge(db));
        }
    }
}