using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;

namespace Darren.Security.Controllers
{
    [Route("movies")]
    [EnableCors("darren")]
    public class MoviesController : Controller
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Content("list o movies");
        }

        [HttpGet("{id}")]
        [DisableCors]
        public IActionResult Get(int id)
        {
            return Content($"Movie {id}");
        }
    }
}