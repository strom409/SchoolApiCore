using login.Repository;

namespace Login.Services.Users
{
    public interface IUserService
    {
        Task<ResponseModel> AddUserAsync(RequestUserDto user, string clientId);
    }
}
