using Timetable_Arrangement.Repository;
using Timetable_Arrangement.Services.TimeTableArrangements;

namespace Timetable_Arrangement.Services.TimeTableHistory
{
    public interface ITimeTableHistoryServices
    {
        Task<ResponseModel> AddTimeTableTeacherHistory(TimeTableDto timeTable, string clientId);
        Task<ResponseModel> GetTimeTableTeacherHistory(string teacherId, string dayId, string clientId);
    }
}
