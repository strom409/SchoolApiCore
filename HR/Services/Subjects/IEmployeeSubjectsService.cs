using HR.Repository;

namespace HR.Services.Subjects
{
    public interface IEmployeeSubjectsService
    {
        Task<ResponseModel> GetEmployeeSubjects(string clientId);
        Task<ResponseModel> GetEmployeeSubjectById(string ESID, string clientId);
        Task<ResponseModel> AddEmployeeSubject(EmployeeSubjects dto, string clientId);
        Task<ResponseModel> UpdateEmployeeSubject(EmployeeSubjects Q, string clientId);
        Task<ResponseModel> DeleteEmployeeSubject(string ESID, string clientId);

    }
}
