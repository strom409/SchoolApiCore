using login.Repository;

namespace Login.Services.HT
{
    public interface IHTService
    {
        Task<ResponseModel> getHT(string clientId);
        Task<ResponseModel> UpdateHT(HTModel htData, string clientId);

    }
}
