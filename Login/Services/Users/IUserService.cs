using login.Repository;

namespace Login.Services.Users
{
    public interface IUserService
    {
        Task<ResponseModel> AddUserAsync(RequestUserDto user, string clientId);
        Task<ResponseModel> UpdateUserAsync(RequestUserDto user, string clientId);
        Task<ResponseModel> DeleteUserAsync(int userId, string clientId);
    }
}
