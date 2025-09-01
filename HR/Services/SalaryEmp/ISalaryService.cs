using HR.Repository;

namespace HR.Services.Salary
{
    public interface ISalaryService
    {

        Task<ResponseModel> SalaryReleaseOnDepartments(SalaryModel salary, string clientId);

        Task<ResponseModel> SalaryReleaseOnEmployeeCode(SalaryModel salary, string clientId);
        Task<ResponseModel> UpdateSalaryDetails(EmployeeDetail employeeDetail, string clientId);

        Task<ResponseModel> UpdateSalaryDetailsOnField(List<EmployeeDetail> employeeDetails, string clientId);

        Task<ResponseModel> DeleteSalaryOnEmployeeCode(string sal, string clientId);

        Task<ResponseModel> DeleteSalaryOnDepartments(List<SalaryModel> salaries, string clientId);
        Task<ResponseModel> AddNewLoan(string sal, string clientId);
        Task<ResponseModel> GetEmployeeSalaryToEdit(string eCode, string clientId);

        Task<ResponseModel> GetDemoSalaryOnDepartments(SalaryModel salary, string clientId);


        Task<ResponseModel> GetEmployeeSalaryToEditOnEDID(string param, string clientId);
        Task<ResponseModel> GetEmployeeSalaryToEditOnECode(string param, string clientId);
        Task<ResponseModel> GetEmployeeSalaryToEditOnFieldName(string param, string clientId);
        Task<ResponseModel> GetSalaryDataOnMonthFromSalaryOnDeparts(string param, string clientId);
        Task<ResponseModel> GetCalculatedGrossNetEtc(string param, string clientId);
        Task<ResponseModel> GetCalculatedGrossNetEtcOnEDID(string param, string clientId);
        Task<ResponseModel> GetSalaryDataOnYearFromSalaryOnECode(string param, string clientId);
        Task<ResponseModel> GetLoanDefaultList(string clientId);
        Task<ResponseModel> SalaryPaymentAccountStatementOnEcodeAndDates(string param, string clientId);
        Task<ResponseModel> GetAvailableNetSalaryOnMonthFromSalaryAndSalaryPaymentOnDeparts(string param, string clientId);
        Task<ResponseModel> GetBankSalarySlipOnMonthFromSalaryAndSalaryPaymentOnDeparts(string param, string clientId);
    }
}
