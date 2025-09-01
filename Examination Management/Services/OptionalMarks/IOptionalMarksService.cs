using Examination_Management.Repository;

namespace Examination_Management.Services.OptionalMarks
{
    public interface IOptionalMarksService
    {
        Task<ResponseModel> AddOptionalMark(OptionalMarksDto dto, string clientId);
        Task<ResponseModel> UpdateOptionalMarks(OptionalMarksDto dto, string clientId);
        Task<ResponseModel> GetMaxMarksByClassSectionSubjectUnit(string id, string clientId);
        Task<ResponseModel> DeleteOptionalMarks(string id, string clientId);
    }
}
