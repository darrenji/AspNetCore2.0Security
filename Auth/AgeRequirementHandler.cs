using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Darren.Security.Auth
{
    public class AgeRequirementHandler : AuthorizationHandler<AgeRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AgeRequirement requirement)
        {
            if (!context.User.HasClaim(c => c.Type == ClaimTypes.DateOfBirth))
                return Task.CompletedTask;

            var dateOfBirth = DateTime.Parse(context.User.FindFirst(c => c.Type == ClaimTypes.DateOfBirth).Value);
            int calculateAge = DateTime.Today.Year - dateOfBirth.Year;
            if (calculateAge > requirement.Age)
                context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }
}
