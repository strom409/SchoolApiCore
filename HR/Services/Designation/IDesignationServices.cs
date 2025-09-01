using HR.Repository;

namespace HR.Services.Designation
{
    public interface IDesignationServices
    {
        Task<ResponseModel> GetDesignations(string clientId);
        Task<ResponseModel> GetDesignationById(long id, string clientId);
        Task<ResponseModel> AddDesignationAsync(Designations designation, string clientId);
        Task<ResponseModel> UpdateDesignationAsync(Designations designation, string clientId);
        Task<ResponseModel> DeleteDesignationAsync(long id, string clientId);

    }
}
