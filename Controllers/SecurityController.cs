using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Darren.Security.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

namespace Darren.Security.Controllers
{
    public class SecurityController : Controller
    {
        public IActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl ?? "/";
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginInputModel inputModel)
        {
            if (!IsAuthenticate(inputModel.Username, inputModel.Password))
                return View();

            //claims
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, "Hi"),
                new Claim(ClaimTypes.Email, inputModel.Username)
            };

            //identity
            ClaimsIdentity identity = new ClaimsIdentity(claims, "cookie");

            //principal
            ClaimsPrincipal principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                scheme: "DarrenSecurityScheme",
                principal: principal,
                properties: new AuthenticationProperties {
                    //IsPersistent = true, // for remember
                    //ExpiresUtc = DateTime.UtcNow.AddMinutes(1)
                });

            return Redirect(inputModel.RequestPath ?? "/");
        }

        public async Task<IActionResult> Logout(string requestPath)
        {
            await HttpContext.SignOutAsync(scheme: "DarrenSecurityScheme");
            return RedirectToAction("Login");
        }

        public IActionResult Access()
        {
            return View();
        }

        private bool IsAuthenticate(string username, string password)
        {
            return (username == "darren" && password == "yes");
        }
    }
}