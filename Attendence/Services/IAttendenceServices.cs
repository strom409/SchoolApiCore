using Attendence.Repository;

namespace Attendence.Services
{
    public interface IAttendenceServices
    {

        Task<ResponseModel> AddAttendance(AttendanceDTO attendance, string clientId);

        Task<ResponseModel> AddAttendanceList(List<AttendanceDTO> attendanceList, string clientId);

        Task<ResponseModel> UpdateTodaysAttendance(AttendanceDTO attendance, string clientId);
        Task<ResponseModel> GetTodaysAttendance(string sessionId, string date, string clientId);
        Task<ResponseModel> GetEditAttendance(string sectionId, string date, string clientId);
        Task<ResponseModel> GetAbsentList(string sectionId, string date, string clientId);
        Task<ResponseModel> getAttendanceListOnDates(string dateFrom, string dateTo, string Session, string clientId);
        Task<ResponseModel> CheckAttendanceAddedorNot(string classId, string sectionId, string date, string clientId);
        Task<ResponseModel> GetMonthlyAttendance(string session, string dateFrom, string dateTo, string className, string sectionName, string clientId);
        Task<ResponseModel> getAttendanceListOnDateswithclassid(string dateFrom, string dateTo, string classId, string sectionId, string clientId);

        
        Task<ResponseModel> GetPendingAttendanceStudents(string classId, string sectionId, string session, string date, string clientId);
        Task<ResponseModel> GetAttendanceMarkedOrNotAsPerDate(string session, string clientId);

        Task<ResponseModel> GetAttendanceReport(string classId, string sectionId, string dateFrom, string dateTo, string clientId);

    }
}
