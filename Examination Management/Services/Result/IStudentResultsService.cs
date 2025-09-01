using Examination_Management.Repository;

namespace Examination_Management.Services.Result
{
    public interface IStudentResultsService
    {
        Task<ResponseModel> GetStudentResultsAsync(StudentResultsRequestDto request, string clientId);
        Task<ResponseModel> GetOptionalResultsAsync(OptionalResultsRequestDto request, string clientId);
    }
}
