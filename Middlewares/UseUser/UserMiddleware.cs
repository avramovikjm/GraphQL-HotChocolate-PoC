using FirebaseAdminAuthentication.DependencyInjection.Models;
using GraphQLDemo.Models;
using HotChocolate.Resolvers;
using System.Security.Claims;

namespace GraphQLDemo.Middlewares.UseUser
{
    public class UserMiddleware
    {
        private readonly FieldDelegate _next;
        public const string USER_CONTEXT_DATA_KEY = "User";

        public UserMiddleware(FieldDelegate next)
        {
            _next = next;
        }

        public async Task Invoke (IMiddlewareContext context)
        {
            if (context.ContextData.TryGetValue("ClaimsPrincipal", out object? rawClaimsPrincipal) && rawClaimsPrincipal is ClaimsPrincipal claimsPrincipal) 
            {
                var user = new User
                {
                    Id = claimsPrincipal.FindFirstValue(FirebaseUserClaimType.ID),
                    Email = claimsPrincipal.FindFirstValue(FirebaseUserClaimType.EMAIL),
                    EmailVerified = bool.TryParse(claimsPrincipal.FindFirstValue(FirebaseUserClaimType.EMAIL_VERIFIED), out bool emailVerified) && emailVerified,
                    Username = claimsPrincipal.FindFirstValue(FirebaseUserClaimType.USERNAME)
                };

                context.ContextData.Add("User", user);
            }

            await _next(context);
        }
    }
}
