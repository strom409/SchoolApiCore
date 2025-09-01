
using login.Repository;

namespace Login.Services.Login
{
    public interface ILoginService
    {
        Task<ResponseModel> LoginAsync(LoginDto request);
        Task<ResponseModel> ValidateToken(string token, string clientId);
    }
}
