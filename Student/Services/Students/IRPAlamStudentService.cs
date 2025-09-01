using Student.Repository;

namespace Student.Services.Students
{
    public interface IRPAlamStudentService
    {
        Task<ResponseModel> GetAllStudentsOnStudentInfoIdAsync(string StudentID, string clientId);
        Task<ResponseModel> GetAttendanceMarkedOrNotAsPerDateAsync(string session, string clientId);
        Task<ResponseModel> GetCopyCheckingOnSectionIdAsync(string session, string clientId);
        Task<ResponseModel> GetPTMReportAsync(string session, string clientId);
    }
}
