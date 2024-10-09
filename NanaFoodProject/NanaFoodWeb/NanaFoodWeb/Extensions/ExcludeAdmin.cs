using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace NanaFoodWeb.Extensions
{
    public class ExcludeAdmin : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;

            // Nếu người dùng đã đăng nhập
            if (user.Identity.IsAuthenticated)
            {
                // Kiểm tra xem người dùng có thuộc role "admin" hay không
                if (user.IsInRole("admin"))
                {
                    // Nếu có role "admin", chặn truy cập
                    context.Result = new ForbidResult(); // Chặn truy cập với role admin
                    return;
                }
            }
        }
    }
}
