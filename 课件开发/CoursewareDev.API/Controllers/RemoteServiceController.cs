using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoursewareDev.BLL;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CoursewareDev.API.Controllers
{
    /// <summary>
    /// 远程服务
    /// </summary>
    [Route("coursewaredev/v1")]
    [ApiController]
    public class RemoteServiceController : ControllerBase
    {
        private readonly pf_courseware_devContext db;
        private readonly RemoteService remote;
        public RemoteServiceController(pf_courseware_devContext db, RemoteService remote)
        {
            this.db = db;
            this.remote = remote;
        }
        [HttpGet("GetPageScript")]
        public JsonResult GetPageScript( long id)
        {
            return new JsonResult(remote.GetPageScript(db, id));
        }
    }
}