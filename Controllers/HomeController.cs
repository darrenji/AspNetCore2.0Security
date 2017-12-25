using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Darren.Security.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index() => Content("Home");

        public IActionResult About() => Content("About");

        public IActionResult Exception() => Content("Exception");

        public IActionResult Error() => Content("Error");

        //Home/GoLocalRedirect?url=/Home/About, About
        //Home/GoLocalRedirect?url=http://www.baidu.com , Exception
        public IActionResult GoLocalRedirect(string url)
        {
            return LocalRedirect(url);
        }


        //Home/GoIsLocalUrl?url=/Home/About, About
        //Home/GoIsLoalUrl?url=http://wwww.baidu.com , Error
        public IActionResult GoIsLocalUrl(string url)
        {
            if (Url.IsLocalUrl(url))
                return Redirect(url);
            else
                return RedirectToAction("Error", "Home");
        }
    }
}