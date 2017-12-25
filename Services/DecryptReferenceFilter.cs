using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Darren.Security.Services
{
    public class DecryptReferenceFilter : IActionFilter
    {
        private readonly IDataProtector protector;

        public DecryptReferenceFilter(IDataProtectionProvider provider)
        {
            this.protector = provider.CreateProtector("protect_my_query_string");
        }
        public void OnActionExecuted(ActionExecutedContext context)
        {
           
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            object param = context.RouteData.Values["id"].ToString();
            var id = int.Parse(this.protector.Unprotect(param.ToString()));
            context.ActionArguments["id"] = id;
        }
    }

    public class DecryptReferenceAttribute:TypeFilterAttribute
    {
        public DecryptReferenceAttribute():base(typeof(DecryptReferenceFilter))
        {

        }
    }
}
