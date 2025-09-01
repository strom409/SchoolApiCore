using HR.Repository;
using HR.Services.Salary;

namespace HR.Services.GrossPay
{
    public interface IGrossPayService
    {
        Task<ResponseModel> UpdateSalaryDetailsOnGrossPay(List<GrossPaySalary> salaries);
        Task<ResponseModel> CalculateSalaryDetailsOnGrossPay(string employeeCode, string clientId);
        Task<ResponseModel> GetSalaryDetailsOnGrossPay(string year, string clientId);
        Task<ResponseModel> GetSalaryDetailsOnGrossPayEDID(string edid, string clientId);
        Task<ResponseModel> UpdateSalaryDetailsOnGrossPay(List<GrossPaySalary> salaries, string clientId);
    }
}
