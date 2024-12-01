using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using NanaFoodWeb.IRepository;
using NanaFoodWeb.Models;
using NanaFoodWeb.Models.Dto;
using Newtonsoft.Json;

namespace NanaFoodWeb.Extensions
{
    public class EmailConfirmed : IAsyncActionFilter
    {
        private readonly IAuthRepository _authRepo;
        private readonly ITokenProvider _tokenProvider;

        // Inject repository vào constructor
        public EmailConfirmed(IAuthRepository authRepo, ITokenProvider tokenProvider)
        {
            _authRepo = authRepo;
            _tokenProvider = tokenProvider;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var token = _tokenProvider.GetToken();
            var userId = _tokenProvider.ReadToken("nameid", token);
            var email = _tokenProvider.ReadToken("email", token);
            var fullName = _tokenProvider.ReadToken("name", token);
            var userName = _tokenProvider.ReadToken("given_name", token);

            UserReturn userReturn = new UserReturn()
            {
                UserName = userName,
                Email = email,
                FullName = fullName,
                Token = token,
            };

            ResponseDto response = new ResponseDto()
            {
                IsSuccess = true,
                Message = "",
                Result = userReturn
            };

            var checkEmailConfirmResponse = await _authRepo.CheckEmailConfirm(userId);

            if (!checkEmailConfirmResponse.IsSuccess)
            {
                var controller = context.Controller as Controller;
                if (controller != null)
                {
                    controller.TempData["response"] = JsonConvert.SerializeObject(response);
                }

                context.Result = new RedirectToActionResult("NotificationConfirmEmail", "Auth", null);
                return;
            }

            await next(); // Tiếp tục thực thi action nếu email đã được xác nhận
        }
    }
}
