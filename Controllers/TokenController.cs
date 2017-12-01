using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Darren.Security.Models;
using Darren.Security.Services;

namespace Darren.Security.Controllers
{
    [Route("token")]
    [AllowAnonymous]
    public class TokenController : Controller
    {
        [HttpPost]
        public IActionResult Create([FromBody]LoginInputModel inputModel)
        {
            if(inputModel.Username!="darren" && inputModel.Password!="yes")
            {
                return Unauthorized();
            }

            var token = new JwtTokenBuilder()
                .AddSecurityKey(JwtSecurityKey.Create("darren-security-key")) //这里的security key与Startup.cs中对应
                .AddSubject("darren")
                .AddIssuer("Darren.Security.Bearer")
                .AddAudience("Darren.Security.Bearer")
                .AddClaim("MembershipId", "111")
                .AddExpiry(1)
                .Build();

            return Ok(token.Value);
        }
    }
}