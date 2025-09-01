using Transport.Repository;

namespace Transport.Services
{
    public interface ITransportService
    {
        Task<ResponseModel> AddTransport(TransportDTO transport, string clientId);
        Task<ResponseModel> AddBusStops(TransportDTO transport, string clientId);
        Task<ResponseModel> UpdateTransport(TransportDTO transport, string clientId);
        Task<ResponseModel> UpdateBusStops(TransportDTO transport, string clientId);
        Task<ResponseModel> UpdateBusStopsLatLong(TransportDTO transport, string clientId);
        Task<ResponseModel> UpdateBusStopRates(StudentBusReportDTO sbr, string clientId);
        Task<ResponseModel> updateroute(TransportDTO transport, string clientId);
        Task<ResponseModel> UpdateStudentRouteAndBusStop(StudentBusReportDTO sbr, string clientId);
        Task<ResponseModel> aupdateBusStop(BusStop bs, string clientId);
        Task<ResponseModel> GetTransportListOnSession(string sessionId, string clientId);
        Task<ResponseModel> GetTransportList(string routeId, string clientId);
        Task<ResponseModel> GetTransportListRateFromInfo(string routeId, string clientId);
        Task<ResponseModel> GetTransportListWithBusRate(string routeId, string clientId);
        Task<ResponseModel> GetTransportByRouteId(string routeId, string clientId);
        Task<ResponseModel> GetStudentRouteDetails(string param, string clientId);
        Task<ResponseModel> GetStopListByName(string stopName, string clientId);
        Task<ResponseModel> GetAllStops(string clientId);
        Task<ResponseModel> GetClassIdsAssigned(string userId, string clientId);
        Task<ResponseModel> GetAssignedSections(string param, string clientId);
        Task<ResponseModel> GetStudentBusReportListOnSectionID(string sectionId, string clientId);
        Task<ResponseModel> GetStudentListOnRouteID(string routeId, string clientId);
        Task<ResponseModel> GetStudentBusRateClasswise(string classId, string clientId);
        Task<ResponseModel> GetStudentBusRate(string param, string clientId);
        Task<ResponseModel> GetTransportNameById(string routeId, string clientId);
        Task<ResponseModel> getTransportList(string clientId);
        Task<ResponseModel> GetStopListWithLatLong(string routeId, string clientId);
        Task<ResponseModel> deleteTransport(string routeId, string clientId);
        Task<ResponseModel> DeleteBusStop(string busStopId, string clientId);


    }


}

