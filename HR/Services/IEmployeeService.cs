using HR.Repository;
using System.Data;

namespace HR.Services
{
    public interface IEmployeeService
    {
        Task<ResponseModel> AddNewEmployee(EmployeeDetail emp, string clientId);

        Task<ResponseModel> UpdateEmployee(EmployeeDetail value, string clientId);
        Task<ResponseModel> UpdateMultipleEmployee(EmployeeDetail value, string clientId);
        Task<ResponseModel> UpdateEmployeeMonthlyAttendance(EmployeeDetail value, string clientId);
        Task<ResponseModel> WithdrawEmployee(EmployeeDetail value, string clientId);
        Task<ResponseModel> RejoinEmployee(EmployeeDetail value, string clientId);
        Task<ResponseModel> UpdateEmployeeDetailField(EmployeeDetail value, string clientId);

        Task<ResponseModel> GetEmployeeByCode(string empCode, string clientId);
        Task<ResponseModel> GetAllEmployeesByYear(string year, string clientId);
        Task<ResponseModel> GetEmployeesBySubDept(string param, string clientId);
        Task<ResponseModel> GetEmployeesByDesignation(string param, string clientId);
        Task<ResponseModel> GetEmployeesByStatus(string param, string clientId);
        Task<ResponseModel> GetEmployeesByName(string param, string clientId);
        Task<ResponseModel> GetEmployeesByField(string param, string clientId);
        Task<ResponseModel> GetEmployeesByMobile(string param, string clientId);
        Task<ResponseModel> GetEmployeesByParentage(string param, string clientId);
        Task<ResponseModel> GetEmployeesByAddress(string param, string clientId);
        Task<ResponseModel> GetEmployeesForAttendanceUpdate(string param, string clientId);
        Task<ResponseModel> GetEmployeeTableFields(string clientId);
        Task<ResponseModel> GetNextEmployeeCode(string clientId);
        Task<List<EmployeeDetail>> EmpData(DataSet ds, string clientId);

    }
}
