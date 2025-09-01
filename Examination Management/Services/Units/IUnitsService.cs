using Examination_Management.Repository;

namespace Examination_Management.Services.Units
{
    public interface IUnitsService
    {
        Task<ResponseModel> AddUnit(UnitDto unit, string clientId);
        Task<ResponseModel> UpdateUnit(UnitDto unit, string clientId);
        Task<ResponseModel> GetAllUnits(string clientId);
        Task<ResponseModel> GetUnitById(string? param, string clientId);
    }
}
