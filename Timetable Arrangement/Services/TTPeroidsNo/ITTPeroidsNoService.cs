using Timetable_Arrangement.Repository;

namespace Timetable_Arrangement.Services.TTPeroids
{
    public interface ITTPeroidsNoService
    {
        Task<ResponseModel> PeroidName(string pid, string clientId);
    }
}
