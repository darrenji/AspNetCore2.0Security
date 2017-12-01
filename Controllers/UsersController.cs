using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Darren.Security.Services.Identity;

namespace Darren.Security.Controllers
{
    [Authorize]
    public class UsersController : Controller
    {
        private readonly UserManager<AppIdentityUser> userManager;

        public UsersController(UserManager<AppIdentityUser> userManager)
        {
            this.userManager = userManager;
        }
        public IActionResult Index()
        {
            var viewModel = this.userManager.Users.ToList();
            return View(viewModel);
        }
    }
}