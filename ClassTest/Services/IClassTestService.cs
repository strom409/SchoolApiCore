using ClassTest.Repository;

namespace ClassTest.Services
{
    public interface IClassTestService
    {
        Task<ResponseModel> AddClassTestMaxMarks(List<ClassTestDTO> list, string clientId);
        Task<ResponseModel> AddClassTestMarks(List<ClassTestDTO> list, string clientId);
        Task<ResponseModel> UpdateClassTestMaxMarks(List<ClassTestDTO> list, string clientId);
        Task<ResponseModel> EditUpdateClassTestMarks(List<ClassTestDTO> list, string clientId);
        Task<ResponseModel> GetSubjectForMaxMarks(string param, string clientId);
        Task<ResponseModel> GetMaxMarks(string param, string clientId);
        Task<ResponseModel> GetStudents(string param, string clientId);
        Task<ResponseModel> GetStudentsWithMarks(string param, string clientId);
        Task<ResponseModel> ViewDateWiseResult(string param, string clientId);
        Task<ResponseModel> ClassTestReport(string param, string clientId);
        Task<ResponseModel> ViewDateWiseResultForAllSubjects(string param, string clientId);
        Task<ResponseModel> ViewDateWiseResultForTotalMarks(string param, string clientId);
        Task<ResponseModel> ViewDateWiseTotalMMandObtMarks(string param, string clientId);
        Task<ResponseModel> GetMissingClassTestMarks(string param, string clientId);

    }

}
