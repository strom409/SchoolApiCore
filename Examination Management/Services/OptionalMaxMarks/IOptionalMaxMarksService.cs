using Examination_Management.Repository;
using Examination_Management.Services.OptionalMarks;

namespace Examination_Management.Services.OptionalMaxMarks
{
    public interface IOptionalMaxMarksService
    {
        Task<ResponseModel> AddOptionalMaxMarks(OptionalMaxMarksDto dto, string clientId);
        Task<ResponseModel> UpdateOptionalMaxMarks(OptionalMaxMarksDto dto, string clientId);
        Task<ResponseModel> GetOptionalMaxMarksByFilter(string param, string clientId);
    }
}
