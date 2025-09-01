using Timetable_Arrangement.Repository;

namespace Timetable_Arrangement.Services.TTAssignPeroids
{
    public interface ITTAssignPeroidsService
    {
        // Assign a timetable (Add)
        Task<ResponseModel> AssignTimetable(TimeTable td, string clientId);

        // Get timetable assigned to a specific teacher
        Task<ResponseModel> GetassignedTT(string teacherId, string clientId);

        // Get the complete timetable for all teachers/classes
        Task<ResponseModel> Getwholetimetable(string clientId);

        // Get detailed timetable for a specific teacher
        Task<ResponseModel> GetTeacherTimeTable(string teacherId, string clientId);

        // Get timetable filtered by current session
        Task<ResponseModel> GetTimeTableWithCurrentSession(string session, string clientId);

        // Swap timetable between two teachers
        Task<ResponseModel> SwapTimeTable(TimeTable ttval, string clientId);

        // Update an assigned timetable
        Task<ResponseModel> UpdateAssignedTimetable(TimeTable ttval, string clientId);
    }
}

