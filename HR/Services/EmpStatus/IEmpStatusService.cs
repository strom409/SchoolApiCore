using HR.Repository;

namespace HR.Services.EmpStatus
{
    public interface IEmpStatusService
    {
        Task<ResponseModel> GetEmployeeStatus(string clientId);
    }
}
