using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataStatistic.BLL;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace DataStatistic.API.Controllers
{
    /// <summary>
    /// 考试测评
    /// </summary>
    [Route("statistic/v1")]
    [ApiController]
    public class ExamController : ControllerBase
    {
        private readonly IHttpClientHelper client;
        private readonly Exam exam;
        private readonly IConfiguration configuration;
        private readonly pf_datastatisticContext db;
        public ExamController(IHttpClientHelper client, Exam exam,IConfiguration configuration, pf_datastatisticContext db)
        {
            this.client = client;
            this.exam = exam;
            this.configuration = configuration;
            this.db = db;
        }
        /// <summary>
        /// 获取考试正确率排名
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetExamCorrectRank")]
        public JsonResult GetExamCorrectRank()
        {
            string str = configuration["DicUrl"];
            //部门
            string strDepartment = client.GetRequest(str + "department_type").Result;
            DicModel dicDepartment = Newtonsoft.Json.JsonConvert.DeserializeObject<DicModel>(strDepartment);
            return new JsonResult(exam.GetExamCorrectRank(db,dicDepartment));
        }

        /// <summary>
        /// 获取考试通过率排名
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetExamPassRate")]
        public JsonResult GetExamPassRate()
        {
            string str = configuration["DicUrl"];
            //部门
            string strDepartment = client.GetRequest(str + "department_type").Result;
            DicModel dicDepartment = Newtonsoft.Json.JsonConvert.DeserializeObject<DicModel>(strDepartment);
            return new JsonResult(exam.GetExamPassRate(db,dicDepartment));
        }

        /// <summary>
        /// 获取考试知识点掌握度
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetExamKnowledge")]
        public JsonResult GetExamKnowledge()
        {
            return new JsonResult(exam.GetExamKnowledge(db));
        }
    }
}