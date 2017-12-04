using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Darren.Security.Models.Security;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

namespace Darren.Security.Controllers
{
    [AllowAnonymous]
    public class SecurityController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginInputModel inputModel)
        {
            //这里肯定通过，来到下一步
            if (!IsAuthentic(inputModel.Username, inputModel.Password))
                return View();

            //claims
            var claims = GetClaims(inputModel.Username);

            //identity
            var identity = new ClaimsIdentity(claims, "cookie");

            //principal
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                    scheme: "DarrenSecurityScheme",
                    principal: principal);

            return RedirectToAction("Index", "Home");
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

        private bool IsAuthentic(string username, string password)
        {
            return  !string.IsNullOrEmpty(username);
        }

        private List<Claim> GetClaims(string username)
        {
            if(username.ToLower() == "free")
            {
                return new List<Claim>
                {
                    new Claim(ClaimTypes.Name, "Free Member"),
                    new Claim("MembershipId", "111")
                };
            }

            if(username.ToLower() == "over18")
            {
                return new List<Claim>
                {
                    new Claim(ClaimTypes.Name, "Over 18"),
                    new Claim("MembershipId", "333"),
                    new Claim("HasCreditCard", "Y"),
                    new Claim(ClaimTypes.DateOfBirth, "01/01/1980"),
                    new Claim("AllowNewReleases", "false")
                };
            }

            return new List<Claim>
            {
                new Claim(ClaimTypes.Name, "Guest")
            };
        }
    }
}