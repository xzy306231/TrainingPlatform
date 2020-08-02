using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataStatistic.BLL;
using Microsoft.AspNetCore.Mvc;

namespace DataStatistic.API.Controllers
{
    [Route("statistic/v1")]
    [ApiController]
    public class CrudController : ControllerBase
    {
        [HttpGet("AAA")]
        public JsonResult Get()
        {
            return new JsonResult(new Crud().Get());
        }

        [HttpPost("Create")]
        public JsonResult Create()
        {
            return new JsonResult(new Crud().Create());
        }
        [HttpPost("Create1")]
        public JsonResult Create1()
        {
            return new JsonResult(new Crud().Create1());
        }
    }
}
