using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace developer.mapserv.utah.gov.Areas.Secure.Controllers
{
    [Area("secure")]
    [Route("~/secure")]
    public class HomeController : Controller
    {
        // GET: /<controller>/
        [HttpGet]
        [Route("")]
        public ViewResult Index()
        {
            return View();
        }

        [HttpGet]
        [Route("keymanagement")]
        public ViewResult KeyManagement() {
            return View();
        }

        [HttpGet]
        [Route("generatekey")]
        public ViewResult GenerateKey() {
            return View();
        }
    }
}
