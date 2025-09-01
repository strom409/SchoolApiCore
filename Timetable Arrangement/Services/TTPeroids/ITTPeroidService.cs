using Timetable_Arrangement.Repository;
using Timetable_Arrangement.Services.TTAssignPeroids;

namespace Timetable_Arrangement.Services.TTPeroids
{
    public interface ITTPeroidService
    {
        Task<ResponseModel> addTTperoidtime(TimeTable ttval, string clientId);
        Task<ResponseModel> updateTTperoidtime(TimeTable ttval, string clientId);
        Task<ResponseModel> Gettimetable(string clientId);
        Task<ResponseModel> getPeroidNo(string clientId);
        Task<ResponseModel> getTimeTablePeriodsWithDuration(string clientId);
        Task<ResponseModel> GetPeriodList(string pid, string clientId);
    }
}
