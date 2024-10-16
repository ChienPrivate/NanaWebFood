namespace NanaFoodWeb.Models.Dto.ViewModels
{
    public class ChangePassAndUserDto
    {
        public UserDto UserDto { get; set; }
        public ChangePasswordDto? changepass {get; set;}
    }
}
