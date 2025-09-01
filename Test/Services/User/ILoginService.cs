using Student.Repository;
using Test.Services.User;

namespace Student.Services.User
{
    public interface ILoginService
    {
        Task<ResponseModel> LoginAsync(LoginDto request);
        Task<ResponseModel> CompareFacesAsync(CompareFacesRequestDTO request);
    }
}
