using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Darren.Security.Controllers
{
    [Authorize]
    public class MoviesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}