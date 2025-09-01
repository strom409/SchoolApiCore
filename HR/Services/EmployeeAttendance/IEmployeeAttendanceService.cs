using HR.Repository;

namespace HR.Services.EmployeeAttendance
{
    public interface IEmployeeAttendanceService
    {
        Task<ResponseModel> GetEmpAttendanceByCodeOnDateRange(string employeeCode, string ClientId);
        Task<ResponseModel> AddEmployeeAttendance(EmpAttendance value, string clientId);
        Task<ResponseModel> UpdateEmployeeAttendance(EmpAttendance value, string clientId);
        Task<ResponseModel> DeleteEmployeeAttendance(EmpAttendance value, string clientId);
    }
}
