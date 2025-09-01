using HR.Repository;

namespace HR.Services.SalaryPayment
{
    public interface ISalaryPaymentService
    {
        Task<ResponseModel> AddSalaryPayment(List<SalaryPayment> payments, string clientId);
        Task<ResponseModel> GetSalaryPaymentStatementOnMonthAndYear(string? param, string clientId);
        Task<ResponseModel> DeleteSalaryPayment(List<SalaryPayment> payments, string clientId);

    }
}
