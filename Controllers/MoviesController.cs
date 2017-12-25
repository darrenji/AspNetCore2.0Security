﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;

namespace Darren.Security.Controllers
{
    [RequireHttps]
    public class MoviesController : Controller
    {
        public IActionResult Index()
        {
            return Content("movies");
        }
    }
}