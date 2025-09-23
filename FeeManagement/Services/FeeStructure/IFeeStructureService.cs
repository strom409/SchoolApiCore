using FeeManagement.Repository;

namespace FeeManagement.Services.FeeStructure
{
    public interface IFeeStructureService
    {
        Task<ResponseModel> AddFeeStructure(FeeStructureDto dto, string clientId);
        Task<ResponseModel> UpdateFeeStructure(FeeStructureDto dto, string clientId);
        Task<ResponseModel> GetAllFeeStructures(string clientId);
        Task<ResponseModel> GetFeeStructureById(long fsId, string clientId);
        Task<ResponseModel> DeleteFeeStructure(long fsId, string updateBy, string clientId);
        Task<ResponseModel> GetFeeStructuresByClassId(long cIDFK, string clientId);
    }
}
