using System;
using System.IO;
using KnowledgeTag.BLL;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace KnowledgeTag.API.Controllers
{
    [Route("knowledgeTag/v1")]
    [ApiController]
    public class knowledgeTagController : ControllerBase
    {
        private readonly IConfiguration configuration;
        private readonly KnowledgeTagManagement knowledge;
        private readonly IHttpClientHelper client;
        private readonly pf_knowledge_tagContext db;
        private readonly RabbitMQClient rabbit;
        public knowledgeTagController(KnowledgeTagManagement knowledge, IHttpClientHelper client, pf_knowledge_tagContext db, RabbitMQClient rabbit, IConfiguration configuration)
        {
            this.knowledge = knowledge;
            this.client = client;
            this.db = db;
            this.rabbit = rabbit;
            this.configuration = configuration;
        }
        /// <summary>
        /// 获取知识体系
        /// </summary>
        /// <param name="TagName"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetTagTree(string TagName)
        {
            return new JsonResult(knowledge.GetTagTree(TagName,db));
        }

        [HttpGet("GetTagTree")]
        public JsonResult GetTagTree()
        {
            string TagName = "";
            return new JsonResult(knowledge.GetTagTree(TagName,db));
        }
        /// <summary>
        /// Excel导入
        /// </summary>
        /// <param name="file"></param>
        [HttpPost("ImportExcel")]
        public JsonResult ImportExcel(IFormFile file)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "excel/";
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            string savePath = Path.Combine(path, Guid.NewGuid() + ".xlsx");
            using (FileStream fs = System.IO.File.Create(savePath))
            {
                file.CopyTo(fs);
                fs.Flush();
            }
            return new JsonResult(knowledge.ImportExcel(db,savePath));
        }

        /// <summary>
        /// 添加根节点
        /// </summary>
        /// <param name="TagName">节点名称</param>
        /// <param name="CreateID">创建人</param>
        ///  <param name="index">索引值</param>
        [HttpPost("AddParentTag")]
        public JsonResult AddParentTag(string TagName, int CreateID,int index)
        {
            var requestJWT = Request.Headers["Authorization"];
            TokenModel obj = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenModel>(client.GetTokenJson(requestJWT).Result);
            return new JsonResult(knowledge.Add_ParentTag(db,rabbit,TagName, CreateID,index,obj));
        }
        /// <summary>
        /// 获取知识点数量
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetTagCount")]
        public object GetTagCount()
        {
            return new JsonResult(knowledge.GetTagCount(db));
        }
        /// <summary>
        /// 获取一级知识点
        /// </summary>
        /// <param name="TagName"></param>
        /// <returns></returns>
        [HttpGet("GetParentTag")]
        public JsonResult GetParentTag(string TagName)
        {
            return new JsonResult(knowledge.GetParentTag(db,TagName));
        }
        /// <summary>
        /// 获取子集
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("GetChildrenTag")]
        public object GetChildrenTag(long id)
        {
            return new JsonResult(knowledge.GetChildrenTag(db,id));
        }
        /// <summary>
        /// 节点拖动
        /// </summary>
        /// <param name="CurrentID">当前拖动的节点</param>
        /// <param name="DestinationID">目的地父节点</param>
        /// <returns></returns>
        [HttpPut("TreeNodeSort")]
        public JsonResult TreeNodeSort(long CurrentID, long DestinationID)
        {
            return new JsonResult(knowledge.TreeNodeSort(db,CurrentID,DestinationID));
        }

        /// <summary>
        /// 添加子节点
        /// </summary>
        /// <param name="TagName">节点名</param>
        /// <param name="ParentID">父节点ID</param>
        /// <param name="CreateID">创建人ID</param>
        /// <param name="index">索引</param>
        [HttpPost("AddNodeTag")]
        public JsonResult AddNodeTag(string TagName, int ParentID, int CreateID,int index)
        {
            var requestJWT = Request.Headers["Authorization"];
            TokenModel obj = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenModel>(client.GetTokenJson(requestJWT).Result);
            return new JsonResult(knowledge.Add_NodeTag(db,rabbit,TagName, ParentID, CreateID,index,obj));
        }

        /// <summary>
        /// 树状图排序
        /// </summary>
        /// <param name="tagList"></param>
        /// <returns></returns>
        [HttpPut("TreeSort")]
        public JsonResult TreeSort(TagList tagList)
        {
            return new JsonResult(knowledge.TreeSort(db,tagList));
        }

        /// <summary>
        /// 编辑节点
        /// </summary>
        /// <param name="TagName">节点名</param>
        /// <param name="NodeID">节点ID</param>
        /// <param name="UpdateBy">修改人ID</param>
        [HttpPut("EditNodeTag")]
        public JsonResult EditNodeTag(string TagName, int NodeID, int UpdateBy)
        {
            var requestJWT = Request.Headers["Authorization"];
            TokenModel obj = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenModel>(client.GetTokenJson(requestJWT).Result);
            return new JsonResult(knowledge.Update_NodeTag(db,rabbit,TagName, NodeID, UpdateBy,obj));
        }

        /// <summary>
        /// 删除节点
        /// </summary>
        /// <param name="ID">删除的节点ID</param>
        [HttpDelete("RemoveTag")]
        public JsonResult RemoveTag(int ID)
        {       
            var requestJWT = Request.Headers["Authorization"];
            TokenModel obj = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenModel>(client.GetTokenJson(requestJWT).Result);
            return new JsonResult(knowledge.Delete_Tag(db,rabbit,ID,obj));         
        }
    }

}