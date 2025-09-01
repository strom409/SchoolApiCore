using HR.Repository;
using HR.Services.Salary;

namespace HR.Services.PayRoll
{
    public interface IPayRollService
    {
        Task<ResponseModel> PayLoan(LoanPayment request, string clientId);
        Task<ResponseModel> PayCPFLoan(LoanPayment request, string clientId);
        Task<ResponseModel> SetLeavesTaken(string clientId);
        Task<ResponseModel> IssueNewLoan(SalaryModel SR, string clientId);
        Task<ResponseModel>IssueCPFLoan(SalaryModel SR, string clientId);
        Task<ResponseModel> GetLoanDetails(long employeeID, string clientId);
        Task<ResponseModel> GetCPFLoanDetails(long employeeID, string clientId);
        Task<ResponseModel> GetCPFLoanDefaultList(string clientId);
        Task<ResponseModel> GetLoanDefaultList(string clientId);
    }
}
