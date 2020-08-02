using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TrainSubject.BLL;
using TrainSubject.Model;

namespace TrainSubject.API.Controllers
{
    /// <summary>
    /// 训练科目
    /// </summary>
    [Route("trainsubject/v1")]
    [ApiController]
    public class TrainSubjectController : ControllerBase
    {
        private readonly IHttpClientHelper client;
        private readonly TrainSubjectManagement trainSubjectManagement;
        public TrainSubjectController(IHttpClientHelper client, TrainSubjectManagement trainSubjectManagement)
        {
            this.trainSubjectManagement = trainSubjectManagement;
            this.client = client;
        }

        /// <summary>
        /// 获取训练科目
        /// </summary>
        /// <param name="queryCriteria">查询条件</param>
        /// <returns></returns>
        [HttpPut("GetTrainSubject")]
        public JsonResult GetTrainSubject(QueryCriteria queryCriteria)
        {
            return new JsonResult(trainSubjectManagement.GetTrainSubject(queryCriteria));
        }

        /// <summary>
        /// 创建训练科目
        /// </summary>
        /// <param name="model">训练科目对象</param>
        /// <returns></returns>
        [HttpPost("CreateTrainSubject")]
        public JsonResult CreateTrainSubject(TrainSubjectModel model)
        {
            var requestJWT = Request.Headers["Authorization"];
           // string str = "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJ1c2VyTm8iOiJ6aHV5dTc3NyIsImV4cCI6MTU3MTk4NTIyMH0.kEHV2kqBO4zEYUljI1doLZglM8nCMDAXtIkHP9mE76U";
            TokenModel obj = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenModel>(client.GetTokenJson(requestJWT).Result);
            return new JsonResult(trainSubjectManagement.CreateTrainSubject(model,obj));
        }

        /// <summary>
        /// Excel导入
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost("TrainSubjectExcelImport")]
        public JsonResult TrainSubjectExcelImport(IFormFile file)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "excel/";
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);//创建本地路径
            string savePath = Path.Combine(path, Guid.NewGuid() + ".xlsx");//全路径
            using (FileStream fs = System.IO.File.Create(savePath))
            {
                file.CopyTo(fs);
                fs.Flush();
            }
            return new JsonResult(trainSubjectManagement.TrainSubjectExcelImport(savePath));
        }

        /// <summary>
        /// 修改训练科目
        /// </summary>
        /// <param name="id">ID</param>
        /// <param name="model">修改的训练科目对象</param>
        /// <returns>True:成功，False失败</returns>
        [HttpPut("UpdateTrainSubject")]
        public JsonResult UpdateTrainSubject(TrainSubjectModel model)
        {
            //return new JsonResult(trainSubjectManagement.UpdateTrainSubject( model));
            var requestJWT = Request.Headers["Authorization"];
            TokenModel obj = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenModel>(client.GetTokenJson(requestJWT).Result);
            return new JsonResult(new TrainSubjectManagement().UpdateTrainSubject( model,obj));
        }

        /// <summary>
        /// 删除训练科目
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("RemoveTrainSubject")]
        public JsonResult RemoveTrainSubject(int id)
        {
            //return new JsonResult(trainSubjectManagement.RemoveTrainSubject(id));
            var requestJWT = Request.Headers["Authorization"];
            TokenModel obj = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenModel>(client.GetTokenJson(requestJWT).Result);
            return new JsonResult(new TrainSubjectManagement().RemoveTrainSubject(id,obj));
        }

        /// <summary>
        /// Excel导出
        /// </summary>
        /// <returns></returns>
        [HttpGet("TrainSubjectExcelExport")]
        public IActionResult TrainSubjectExcelExport()
        {
            var memory = new MemoryStream();
            string path = (string)new TrainSubjectManagement().TrainSubjectExcelExport();
            using (var stream = new FileStream(path, FileMode.Open))
            {
                stream.CopyTo(memory);
            }
            memory.Position = 0;

            return File(memory, "application/vnd.ms-excel", DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx");
        }

    }
}