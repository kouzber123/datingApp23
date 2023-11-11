using API.Extensions;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;

namespace API.Helpers
{
    /// <summary>
    /// /
    /// </summary> <summary>
    ///
    /// </summary>
    public class LogUserActivity : IAsyncActionFilter
    {
        /// <summary>
        /// wait until api has done its job then we update last active property and leave it in baseAPI controller
        /// </summary>
        /// <param name="context"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var resultContext = await next();

            if (!resultContext.HttpContext.User.Identity.IsAuthenticated) return;

            var userId = resultContext.HttpContext.User.GetUserId();
            var repo = resultContext.HttpContext.RequestServices.GetRequiredService<IUserRepository>();

            var user = await repo.GetUserById(userId);
            user.LastActive = DateTime.UtcNow;
            await repo.SaveAllAsync();

        }
    }
}
