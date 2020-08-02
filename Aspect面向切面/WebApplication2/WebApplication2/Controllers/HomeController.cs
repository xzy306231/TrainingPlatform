using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly ICustomService _service;
        public HomeController(ICustomService service)
        {
            _service = service;
        }

        [HttpGet("{str}")]
        public void GetMsg(string str)
        {
            _service.Call(str);
        }

        [HttpGet("GetMsgA")]
        public void GetMsgA(string str)
        {
            _service.CallA(str);
        }
    }
}