using HR.Repository;

namespace HR.Services.Deparments
{
    public interface IDepartmentsService
    {
        Task<ResponseModel> AddDepartment(SubDepartment department, string clientId);
        Task<ResponseModel> UpdateDepartment(SubDepartment department, string clientId);
        Task<ResponseModel> DeleteDepartment(long id, string clientId);
        Task<ResponseModel> getDepartments(string clientId);
        Task<ResponseModel> GetDepartmentById(long id, string clientId);
       
    }
}
