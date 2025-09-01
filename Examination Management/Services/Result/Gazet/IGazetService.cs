using Examination_Management.Repository;

namespace Examination_Management.Services.Result.Gazet
{
    public interface IGazetService
    {
        Task<ResponseModel> GetGazetResults(string param, string clientId);
    }
}
