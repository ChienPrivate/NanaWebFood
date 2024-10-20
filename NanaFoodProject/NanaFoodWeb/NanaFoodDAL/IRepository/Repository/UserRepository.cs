using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NanaFoodDAL.Context;
using NanaFoodDAL.Dto;
using NanaFoodDAL.Dto.UserDTO;
using NanaFoodDAL.Model;

namespace NanaFoodDAL.IRepository.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _context;
        private ResponseDto _response;
        public UserRepository(UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager,
            IMapper mapper,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _mapper = mapper;
            _context = context;
            _response = new ResponseDto();
        }
        public async Task<ResponseDto> CreateUserAsync(CreateUserRequestDto createUserRequestDto)
        {

            if (createUserRequestDto == null)
            {
                _response.IsSuccess = false;
                _response.Result = createUserRequestDto;
                _response.Message = "Dữ liệu đang bị rỗng";
                return _response;
            }

            var userInDb = await _context.Users.FirstOrDefaultAsync(u => u.UserName == createUserRequestDto.UserName);


            if (userInDb != null)
            {
                _response.IsSuccess = false;
                _response.Result = userInDb.UserName;
                _response.Message = $"Tên đăng nhập {userInDb.UserName} đã tồn tại";
                return _response;
            }

            try
            {

                var userMapper = _mapper.Map<User>(createUserRequestDto);

                var userCreationResult = await _userManager.CreateAsync(userMapper);

                if (!userCreationResult.Succeeded)
                {
                    _response.IsSuccess = false;
                    _response.Result = userCreationResult.Errors;
                    _response.Message = "Tạo người dùng thất bại";
                    return _response;
                }

                var result = await _userManager.AddPasswordAsync(userMapper, createUserRequestDto.Password);

                if (!result.Succeeded)
                {
                    var deleteResult = await _userManager.DeleteAsync(userMapper);

                    if (!deleteResult.Succeeded)
                    {
                        _response.IsSuccess = false;
                        _response.Result = deleteResult.Errors;
                        _response.Message = "Thêm mật khẩu thất bại và không thể xóa người dùng";
                    }
                    else
                    {
                        _response.IsSuccess = false;
                        _response.Result = result.Errors;
                        _response.Message = "Thêm mật khẩu thất bại";
                    }

                    return _response;
                }

                if (!await _roleManager.RoleExistsAsync(createUserRequestDto.Role))
                {
                    _response.IsSuccess = false;
                    _response.Message = "Không có vai trò này";
                    return _response;
                }

                // Gán role cho user mới
                await _userManager.AddToRoleAsync(userMapper, createUserRequestDto.Role);

                _response.IsSuccess = true;
                _response.Result = createUserRequestDto;
                _response.Message = "Tạo người dùng thành công";
                return _response;

            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Result = createUserRequestDto;
                _response.Message = ex.Message;

                return _response;
            }
        }

        public async Task<ResponseDto> DeleteUserAsync(string userId)
        {
            if (!string.IsNullOrEmpty(userId))
            {
                // Tìm kiếm người dùng theo Id
                var user = await _userManager.FindByIdAsync(userId);

                if (user != null)
                {
                    // Lấy tất cả các role của người dùng
                    var roles = await _userManager.GetRolesAsync(user);

                    if (roles != null && roles.Any())
                    {
                        // Xóa người dùng khỏi các role
                        var removeRolesResult = await _userManager.RemoveFromRolesAsync(user, roles);

                        if (!removeRolesResult.Succeeded)
                        {
                            _response.IsSuccess = false;
                            _response.Result = removeRolesResult.Errors;
                            _response.Message = "Không thể xóa vai trò của người dùng";
                            return _response;
                        }
                    }

                    // Xóa người dùng
                    var deleteUserResult = await _userManager.DeleteAsync(user);

                    if (!deleteUserResult.Succeeded)
                    {
                        _response.IsSuccess = false;
                        _response.Result = deleteUserResult.Errors;
                        _response.Message = "Không thể xóa người dùng hoặc người dùng không tồn tại";
                    }
                    else
                    {
                        _response.IsSuccess = true;
                        _response.Message = "Xóa người dùng thành công";
                    }

                    return _response;
                }
                else
                {
                    _response.IsSuccess = false;
                    _response.Message = "Người dùng không tồn tại";
                }
            }
            else
            {
                _response.IsSuccess = false;
                _response.Message = "Mã người dùng đang rỗng";
            }

            return _response;
        }

        public async Task<ResponseDto> GetAllRolesAsync()
        {
            try
            {
                _response.IsSuccess = true;
                _response.Result = await _context.Roles.ToListAsync();
                _response.Message = "Lấy danh sách vai trò thành công";
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        public async Task<ResponseDto> GetAllUserAsync()
        {


            try
            {
                var usersWithRoles = await (
                    from user in _context.Users
                    join userRole in _context.UserRoles on user.Id equals userRole.UserId
                    join role in _context.Roles on userRole.RoleId equals role.Id
                    select new UserWithRolesDto
                    {
                        Id = user.Id,
                        AvatarUrl = user.AvatarUrl,
                        FullName = user.FullName,
                        UserName = user.UserName,
                        Status = user.Status,
                        PhoneNumber = user.PhoneNumber,
                        Address = user.Address,
                        Email = user.Email,
                        EmailConfirmed = user.EmailConfirmed,
                        Roles = role.Name  // Lấy tên vai trò từ bảng Roles
                    })
                    .ToListAsync();

                _response.IsSuccess = true;
                _response.Result = usersWithRoles;
                _response.Message = "Lấy danh sách người dùng thành công thành công";


            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Result = null;
                _response.Message = "Lấy danh sách Users không thành công";
            }
            return _response;
        }

        public async Task<ResponseDto> UpdateUserAsync(UpdateUserRequestDto updateUserRequest)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(updateUserRequest.Id);

                if (user == null)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Không tìm thấy người dùng.";
                    return _response;
                }

                user.UserName = updateUserRequest.UserName ?? user.UserName; // Chỉ cập nhật nếu có giá trị mới
                user.Email = updateUserRequest.Email ?? user.Email;
                user.EmailConfirmed = updateUserRequest.EmailConfirmed; // Cập nhật trực tiếp
                user.PhoneNumber = updateUserRequest.PhoneNumber ?? user.PhoneNumber; // Chỉ cập nhật nếu có giá trị mới
                user.FullName = updateUserRequest.FullName ?? user.FullName; // Chỉ cập nhật nếu có giá trị mới
                user.Address = updateUserRequest.Address ?? user.Address; // Chỉ cập nhật nếu có giá trị mới
                user.AvatarUrl = updateUserRequest.AvatarUrl ?? user.AvatarUrl;
                user.Status = updateUserRequest.Status; // Thuộc tính bắt buộc

                if (!string.IsNullOrEmpty(updateUserRequest.Password))
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    await _userManager.ResetPasswordAsync(user, token, updateUserRequest.Password);
                }

                var currentRoles = await _userManager.GetRolesAsync(user);

                if (!string.IsNullOrEmpty(updateUserRequest.Role) && updateUserRequest.Role != currentRoles.First())
                {
                    await _userManager.RemoveFromRolesAsync(user, currentRoles);
                    await _userManager.AddToRoleAsync(user, updateUserRequest.Role);
                }

                // Lưu thay đổi
                _context.Entry(user).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                _response.Result = user;
                _response.Message = "Cập nhật người dùng thành công";
                _response.IsSuccess = true;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = $"Lỗi: {ex.Message}";
            }
            return _response;
        }


        public async Task<ResponseDto> GetUserByIdAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);


            var userInReturn = _mapper.Map<UpdateUserRequestDto>(user);


            if (user != null)
            {
                
                var role = await _userManager.GetRolesAsync(user);

                userInReturn.Role = role.First();

                _response.IsSuccess = true;
                _response.Result = userInReturn;
                _response.Message = "Tìm người dùng thành công";
            }
            else
            {
                _response.IsSuccess = false;
                _response.Result = userInReturn;
                _response.Message = "Không tìm thấy người dùng";
            }

            return _response;
        }
    }
}
