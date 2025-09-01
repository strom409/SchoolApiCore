using Timetable_Arrangement.Repository;
using Timetable_Arrangement.Services.TTAssignPeroids;

namespace Timetable_Arrangement.Services.TimeTableArrangements
{
    public interface ITimeTableArrangementsServices
    {
        Task<ResponseModel> ArrangeTimetable(TimeTableDto td, string clientId);
        Task<ResponseModel> GetTimeTableArrangementsByDate(string onDate, string clientId);
        Task<ResponseModel> GetTimeTableArrangementsOfAbsentTeacherToday(string input, string clientId);
        Task<ResponseModel> DeleteTimetable(TimeTableDelete td, string clientId);
        Task<ResponseModel> ResetTimeTable(string updatedBy, string session, string clientId);
        Task<ResponseModel> DeleteArrangementTimetable(TimeTable td, string clientId);
        Task<ResponseModel> GetEmployeeList(string year, string clientId);
        Task<ResponseModel> GetEmployeeList(string clientId);
        Task<ResponseModel> GetEmployeeListNotInTimeTable(string year, string clientId);
        Task<ResponseModel> GetEmployeeListWhoAreInTimeTable(string year, string clientId);
    }
}
