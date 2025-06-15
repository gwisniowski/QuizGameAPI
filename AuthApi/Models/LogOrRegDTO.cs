namespace AuthApi.Models
{
    public class LoginDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
       
    }
    public class RegisterDto : LoginDto
    {

        public string ConfirmPassword { get; set; }
    }
}