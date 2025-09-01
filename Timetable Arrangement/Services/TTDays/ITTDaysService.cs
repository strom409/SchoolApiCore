using Timetable_Arrangement.Repository;

namespace Timetable_Arrangement.Services.TTDays
{
    public interface ITTDaysService
    {
        Task<ResponseModel> DaydName(string did, string clientId);
        Task<ResponseModel> getweekdays(string clientId);
        Task<ResponseModel> GetwholetimetableProc(string years, string clientId);
    }
}
