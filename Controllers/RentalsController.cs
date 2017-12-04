using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Darren.Security.Models.Rentals;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Darren.Security.Controllers
{
    [Authorize(Policy = "PaidMember")]
    public class RentalsController : Controller
    {
        private readonly IAuthorizationService authService;

        public RentalsController(IAuthorizationService authService)
        {
            this.authService = authService;
        }
        
        
        public IActionResult Rent()
        {
            return View();
        }

        [Authorize(Policy = "Over18")]
        public IActionResult RentOver18()
        {
            return View();
        }

        public async Task<IActionResult> RentNewRelease(Rental inputModel)
        {
            var result = await authService.AuthorizeAsync(User, inputModel, "CanRentNewRelease");
            if (!result.Succeeded)
                return Forbid();
            return View();
        }
    }
}
