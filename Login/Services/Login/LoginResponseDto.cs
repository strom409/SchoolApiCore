namespace Login.Services.Login
{
    public class LoginResponseDto
    {
        public string Token { get; set; }
        public string UserFullName { get; set; }
        public string UserEmail { get; set; }
        public UserDTO User { get; set; }
    }
}
