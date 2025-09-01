namespace Login.Services.Login
{
    public class TokenValidationRequest
    {
        public string Token { get; set; }
        public string ClientId { get; set; }
    }
}
