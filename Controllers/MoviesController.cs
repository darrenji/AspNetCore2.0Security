using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.DataProtection;
using Darren.Security.Models;
using Darren.Security.Services;

namespace Darren.Security.Controllers
{
    [Route("movies")]
    public class MoviesController : Controller
    {
        private readonly IDataProtector protector;
        //private readonly ITimeLimitedDataProtector protector1;

        public MoviesController(IDataProtectionProvider provider)
        {
            this.protector = provider.CreateProtector("protect_my_query_string");
            //this.protector1 = provider.CreateProtector("protect_my_query_string")
            //    .ToTimeLimitedDataProtector();
        }

        [HttpGet]
        public IActionResult Get()
        {
            var model = GetMoVivies();

            var outputModel = model.Select(item => new {
                Id = this.protector.Protect(item.Id.ToString()),//这样获取到的object的id值是被加密的
                //Id = this.protector1.Protect(item => item.Id.ToString(), TimeSpan.FromSeconds(10)),
                item.Title,
                item.ReleaseYear,
                item.Summary
            });

            return Ok(outputModel);
        }

        [HttpGet("{id}")]
        [DecryptReference]
        public IActionResult Get(int id)
        {
            var model = GetMoVivies();
            var outputModel = model.Where(item => item.Id == id);
            return Ok(outputModel);
        }


        public List<Movie> GetMoVivies()
        {
            return new List<Movie>
            {
                new Movie{Id=1,Title="title1", ReleaseYear=1983, Summary="summ1"}
            };
        }
    }
}