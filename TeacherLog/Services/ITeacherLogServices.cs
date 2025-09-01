using TeacherLog.Repository;

namespace TeacherLog.Services
{
    public interface ITeacherLogServices
    {
        Task<ResponseModel> AddTeacherLogDataOnSectionIDandDate(TeacherLogData td, string clientId);
        Task<ResponseModel> AddTeacherPerformance(TeacherLogData td, string clientId);
        Task<ResponseModel> AddTeacherLogForNewTiny(TeacherLogData td, string clientId);
        Task<ResponseModel> UpdateTeacherLog(TeacherLogData trd, string clientId);
        Task<ResponseModel> GetTeacherLogDataOnSectionIDandDate(string sectionIdAndDate, string clientId);
        Task<ResponseModel> GetTeacherLogDataOnECodeandDate(string eCodeAndDate, string clientId);
        Task<ResponseModel> GetSubjectList(string classId, string clientId);
        Task<ResponseModel> GetSubSubjectList(string subjectId, string clientId);
        Task<ResponseModel> GetOptSubjectList(string classId, string clientId);
        Task<ResponseModel> GetTeacherLogOnDateList(string date, string clientId);
        Task<ResponseModel> GetTeacherLogOnDateAndCodeList(string empcode, string clientId);
        Task<ResponseModel> GetTeachersLog(string date, string clientId);
        Task<ResponseModel> GetTeachersLogfromTT(string date, string clientId);
        Task<ResponseModel> GetTeachersLogFromTTEmpty(string date, string clientId);
        Task<ResponseModel> GetTeacherPerformance(string clientId);
        Task<ResponseModel> GetTeachersLogRangeWise(string dateRange, string clientId);
        Task<ResponseModel> DeleteTeacherLogOnLogID(TeacherLogData td, string clientId);

    }
}
