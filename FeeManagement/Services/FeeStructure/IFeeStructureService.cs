using FeeManagement.Repository;

namespace FeeManagement.Services.FeeStructure
{
    public interface IFeeStructureService
    {
        Task<ResponseModel> AddFeeStructure(FeeStructureDto dto, string clientId);
        Task<ResponseModel> UpdateFeeStructure(FeeStructureDto dto, string clientId);
        Task<ResponseModel> GetFeeStructureByClassAndFeeHead(long classId, long fhId, string clientId);
        //  Task<ResponseModel> GetAllFeeStructures(string clientId);
        Task<ResponseModel> GetAllFeeStructures(string clientId, string currentSession);
        Task<ResponseModel> GetFeeStructureById(long fsId, string clientId);
        Task<ResponseModel> DeleteFeeStructure(long fsId, string clientId);
        Task<ResponseModel> GetFeeStructuresByClassId(long cIDFK, string clientId);
    }
}
