using Examination_Management.Repository;

namespace Examination_Management.Services.Marks
{
    public interface IMarksService
    {
        Task<ResponseModel> AddMarks(MarksDto marksDto, string clientId);
        Task<ResponseModel> UpdateMarks(MarksDto dto, string clientId);
        Task<ResponseModel> GetMarksWithNames(string param, string clientId);
        Task<ResponseModel> DeleteMarks(string marksId, string clientId);
    }
}
