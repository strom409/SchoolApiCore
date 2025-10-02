using FeeManagement.Repository;

namespace FeeManagement.Services.FeeDue
{
    public interface IFeeDueService
    {
        Task<ResponseModel> AddFeeDue(FeeDueDTO request, string clientId);
        Task<ResponseModel> GetFeeDueByStudentName(string studentName, string clientId);
        Task<ResponseModel> GetFeeDueByAdmissionNo(string param, string clientId);
        Task<ResponseModel> GetFeeDueByClassId(long classId, string clientId);
        Task<ResponseModel> UpdateFeeDue(FeeDueDTO request, string clientId);
        Task<ResponseModel> GetAllMonths(string clientId);
        Task<ResponseModel> DeleteFeeDue(long feeDueID, string clientId);
        Task<ResponseModel> GetFeeDueBySectionId(long sectionId, int feeHeadId, string clientId);
    }
}
