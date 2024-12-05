using Azure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using NanaFoodWeb.IRepository;
using Newtonsoft.Json;

namespace NanaFoodWeb.Extensions
{
    public class CheckUserStatus : IAsyncActionFilter
    {
        private readonly IAuthRepository _authRepository;
        private readonly ITokenProvider _tokenProvider;
        public CheckUserStatus(IAuthRepository authRepository, ITokenProvider tokenProvider)
        {
            _authRepository = authRepository;
            _tokenProvider = tokenProvider;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var token = _tokenProvider.GetToken();
            string userId = string.Empty;


            if (token != null)
            {
                userId = _tokenProvider.ReadToken("nameid", token);
            }

            var userStatusResponse = await _authRepository.GetUserStatus(userId);

            if (userStatusResponse.IsSuccess)
            {
                var userStatus = int.Parse(userStatusResponse.Result.ToString());

                if (userStatus != 0)
                {
                    var controller = context.Controller as Controller;
                    if (controller != null)
                    {
                        controller.TempData["status"] = userStatus;
                    }
                    context.Result = new RedirectToActionResult("UserNotificaton", "Auth", null);
                    return;
                }
            }

            await next();
        }

    }
}
