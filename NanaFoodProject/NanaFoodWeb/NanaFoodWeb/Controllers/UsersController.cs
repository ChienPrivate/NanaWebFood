using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NanaFoodWeb.IRepository;
using NanaFoodWeb.Models;
using NanaFoodWeb.Models.Dto;
using Newtonsoft.Json;

namespace NanaFoodWeb.Controllers
{
    [Route("Users")]
    [Authorize(Roles = "admin,employee")]
    public class UsersController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly IHelperRepository _helperRepository;
        public UsersController(IUserRepository userRepository, IHelperRepository helperRepository)
        {
            _userRepository = userRepository;
            _helperRepository = helperRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var response = await _userRepository.GetAllUserAsync();

            var users = JsonConvert.DeserializeObject<List<UserWithRolesDto>>(response.Result.ToString()) ?? new List<UserWithRolesDto>();

            
            var admin = users.Where(u => u.Roles == "admin").ToList();
            var employee = users.Where(u => u.Roles == "employee").ToList();
            var cumtomer = users.Where(u => u.Roles == "customer").ToList();

            ViewBag.Admin = admin;

            ViewBag.Customer = cumtomer;

            ViewBag.Employee = employee;


            return View();
        }


        [HttpGet("Create")]
        public async Task<IActionResult> Create()
        {
            ViewBag.StatusList = GetStatusSelectList();
            ViewBag.RoleList = await GetRolesSelectListAsync();

            return View(new CreateUserRequestDto());
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create(CreateUserRequestDto createUserRequestDto, IFormFile? UploadFile, string confirmPassword)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.StatusList = GetStatusSelectList();
                ViewBag.RoleList = await GetRolesSelectListAsync();

                return View(createUserRequestDto);
            }

            if (createUserRequestDto.Password != confirmPassword)
            {
                ModelState.AddModelError("Password", "Mật khẩu và mật khẩu xác nhận không trùng khớp");
                return View(createUserRequestDto);
            }

            if (UploadFile != null && UploadFile.Length > 0)
            {
                createUserRequestDto.AvatarUrl = await UploadImageAsync(UploadFile);
            }

            var response = await _userRepository.CreateUserAsync(createUserRequestDto);

            if (response.IsSuccess)
            {
                TempData["success"] = "Thêm người dùng thành công";
                return RedirectToAction("Index");
            }

            TempData["error"] = response.Message;
            return View(createUserRequestDto);
        }

        [HttpGet("Edit/{userId}")]
        public async Task<IActionResult> Edit(string userId)
        {
            ViewBag.StatusList = GetStatusSelectList();
            ViewBag.RoleList = await GetRolesSelectListAsync();

            var response = await _userRepository.GetUserByIdAsync(userId);

            if (response.IsSuccess)
            {
                var user = JsonConvert.DeserializeObject<UpdateUserRequestDto>(response.Result.ToString());
                return View(user);
            }

            TempData["error"] = "Người dùng không tồn tại";
            return RedirectToAction("Index");
        }

        [HttpPost("Edit/{userId}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdateUserRequestDto updateUserRequestDto, IFormFile? UploadFile)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.StatusList = GetStatusSelectList();
                ViewBag.RoleList = await GetRolesSelectListAsync();

                return View(updateUserRequestDto);
            }

            if (UploadFile != null && UploadFile.Length > 0)
            {
                updateUserRequestDto.AvatarUrl = await UploadImageAsync(UploadFile);
            }

            var response = await _userRepository.UpdateUserAsync(updateUserRequestDto);

            if (response.IsSuccess)
            {
                TempData["success"] = "Cập nhật người dùng thành công";
                return RedirectToAction("Index");
            }


            TempData["Error"] = response.Message;
            return View(updateUserRequestDto);

        }

        [HttpGet("Details/{userId}")]
        public async Task<IActionResult> Details(string userId)
        {
            ViewBag.StatusList = GetStatusSelectList();
            ViewBag.RoleList = await GetRolesSelectListAsync();

            var response = await _userRepository.GetUserByIdAsync(userId);

            if (response.IsSuccess)
            {
                var user = JsonConvert.DeserializeObject<UpdateUserRequestDto>(response.Result.ToString());
                return View(user);
            }

            TempData["error"] = "Người dùng không tồn tại";
            return RedirectToAction("Index");
        }


        [HttpGet("Delete/{userId}")]
        public async Task<IActionResult> Delete(string userId)
        {
            ViewBag.StatusList = GetStatusSelectList();
            ViewBag.RoleList = await GetRolesSelectListAsync();

            var response = await _userRepository.GetUserByIdAsync(userId);

            if (response.IsSuccess)
            {
                var user = JsonConvert.DeserializeObject<UpdateUserRequestDto>(response.Result.ToString());
                return View(user);
            }

            TempData["error"] = "Người dùng không tồn tại";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirm(string userId)
        {
            var response = await _userRepository.DeleteUserAsync(userId);

            if (response.IsSuccess)
            {
                TempData["success"] = "Xóa người dùng thành công";
                return RedirectToAction("Index");
            }
            else
            {
                string message = response.Message;
                return NotFound(message);
            }
        }


        private List<SelectListItem> GetStatusSelectList()
        {
            List<UserStatus> statusList = new()
                {
                    UserStatus.Active,
                    UserStatus.Inactive,
                    UserStatus.Delete,
                    UserStatus.Block
                };

            var selectStatusList = new List<SelectListItem>();
            foreach (var item in statusList)
            {
                string displayText = item switch
                {
                    UserStatus.Active => "Hoạt động",
                    UserStatus.Inactive => "Không hoạt động",
                    UserStatus.Delete => "Đã xóa",
                    UserStatus.Block => "Bị khóa",
                    _ => "Không xác định"
                };

                selectStatusList.Add(new SelectListItem
                {
                    Text = displayText,
                    Value = ((int)item).ToString()  // Giá trị là số của enum
                });
            }

            return selectStatusList;
        }

        private async Task<List<SelectListItem>> GetRolesSelectListAsync()
        {
            var response = await _userRepository.GetAllRolesAsync();

            var selectRoleList = new List<SelectListItem>();

            if (response.IsSuccess)
            {
                // Giả sử response trả về một JSON chứa danh sách các roles
                var roles = JsonConvert.DeserializeObject<List<IdentityRole>>(response.Result.ToString());

                // Duyệt qua từng vai trò và thêm vào danh sách selectRoleList
                foreach (var role in roles)
                {
                    string displayText = role.Name switch
                    {
                        "admin" => "Quản trị viên",    // Đổi "admin" thành "Quản trị viên"
                        "customer" => "Khách hàng",    // Đổi "customer" thành "Khách hàng"
                        "employee" => "Nhân viên",
                        _ => role.Name                 // Nếu không nằm trong danh sách, giữ nguyên giá trị
                    };

                    selectRoleList.Add(new SelectListItem
                    {
                        Text = displayText,           // Hiển thị tên vai trò
                        Value = role.Name               // Giá trị là ID của role
                    });
                }
            }

            return selectRoleList;


        }

        private async Task<string> UploadImageAsync(IFormFile? UploadFile)
        {
            string imageUrl = string.Empty;

            var uploadResponse = await _helperRepository.UploadImageAsync(UploadFile);

            imageUrl = uploadResponse.Result?.ToString() ?? "Tải ảnh lên thất bại";

            return imageUrl;
        }
    }
}
