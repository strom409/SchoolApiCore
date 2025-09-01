using Student.Repository;
using System.Threading.Tasks;

namespace Student.Services.District
{
    public interface IDistrictService
    {
        Task<ResponseModel> GetAllDistricts(string clientId);
        Task<ResponseModel> GetDistrictsByStateId(int stateId, string clientId);
        Task<ResponseModel> GetAllStates(string clientId);
    }
}
