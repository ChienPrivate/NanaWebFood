using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NanaFoodWeb.IRepository;
using NanaFoodWeb.IRepository.Repository;
using NanaFoodWeb.Models.Dto;
using NanaFoodWeb.Models.Dto.ViewModels;
using Newtonsoft.Json;

namespace NanaFoodWeb.Controllers
{
    [Authorize(Roles = "admin")]
    public class DashBoardController : Controller
    {
        private readonly IAuthRepository _authRepo;
        private readonly IHelperRepository _helperRepository;
        public DashBoardController(IAuthRepository authRepository, IHelperRepository helperRepository)
        {
            _authRepo = authRepository;
            _helperRepository = helperRepository;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> AdministratorInformation()
        {
            var response = await _authRepo.GetInfo();
            if (response != null && response.IsSuccess)
            {
                var viewmodel = new ChangePassAndUserDto()
                {
                    UserDto = JsonConvert.DeserializeObject<UserDto>(response.Result.ToString()),
                    changepass = null
                };
                return View(viewmodel);
            }
            return RedirectToAction("AccessDenied");
        }

        [HttpPost]
        public async Task<IActionResult> AdministratorInformation(ChangePassAndUserDto viewmodel, IFormFile? UploadFile)
        {
            string imageUrl = string.Empty;
            if (ModelState.IsValid)
            {
                if (UploadFile != null && UploadFile.Length > 0)
                {
                    // Gọi service để upload hình ảnh
                    var uploadResponse = await _helperRepository.UploadImageAsync(UploadFile);

                    imageUrl = uploadResponse.Result?.ToString() ?? "Tải ảnh lên thất bại";

                    if (!string.IsNullOrEmpty(imageUrl))
                    {
                        // Gắn URL của hình ảnh vào model
                        viewmodel.UserDto.AvatarUrl = imageUrl;
                    }
                    else
                    {
                        viewmodel.UserDto.AvatarUrl = "https://placehold.co/300x300";
                    }
                }

                var respone = await _authRepo.UpdateInfo(viewmodel.UserDto);
                if (respone.IsSuccess)
                {
                    TempData["success"] = respone.Message;
                    viewmodel.UserDto = JsonConvert.DeserializeObject<UserDto>(respone.Result.ToString());
                    return View(viewmodel);
                }
            }

            return View(viewmodel);
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword()
        {
            return View();
        }
    }
}
