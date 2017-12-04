using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Darren.Security.Auth
{
    public class AgeRequirement : IAuthorizationRequirement
    {
        public int Age { get; set; }
        public AgeRequirement(int age)
        {
            this.Age = age;
        }
    }
}
