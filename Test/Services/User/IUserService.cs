using Student.Repository;

namespace Student.Services.User
{
    public interface IUserService
    {
        Task<ResponseModel> AddUserAsync(RequestUserDto user, string clientId);
    }
}
