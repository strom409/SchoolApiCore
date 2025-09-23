using FeeManagement.Repository;

namespace FeeManagement.Services.FeeHead
{
    public interface IFeeHeadService
    {
        Task<ResponseModel> AddFeeHead(FeeHeadDto request, string clientId);
        Task<ResponseModel> UpdateFeeHead(FeeHeadDto request, string clientId);
        Task<ResponseModel> GetFeeHeadById(long fHID, string clientId);
        Task<ResponseModel> GetFeeHeadsByType(int fHType, string clientId);
        Task<ResponseModel> GetAllFeeHeads(string clientId);
        Task<ResponseModel> DeleteFeeHead(long fHID, string clientId, string updatedBy);
    }
}
