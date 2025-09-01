using Azure;
using Student.Repository;

namespace Student.Services.Students
{
    public interface ICrescentStudentServics
    {
        Task<ResponseModel> AddNewStudentWithUID(AddStudentRequestDTO request, string clientId);
        Task<ResponseModel> GetAllActiveStudentsOnSectionIDCrescentSchool(string sectionID, string clientId);
        Task<ResponseModel> GetAllDischargedStudentsOnSectionIDCrescentSchool(string sectionID, string clientId);
        Task<ResponseModel> GetAllStudentsOnSectionIDCrescent(string sectionId, string clientId);
        Task<ResponseModel> GetAllStudentsOnClassIDCrescent(string classId, string clientId);
        Task<ResponseModel> GetInvalidDischargeListOnClassIDCrescent(string classId,string clientId);
        Task<ResponseModel> GetMaxUID(string currentSession, string clientId);
        Task<ResponseModel> GetMaxRollNo(string classId, string sectionId, string clientId);
        Task<ResponseModel> GetActiveStudentsOnUID(string UID, string clientId);

        Task<ResponseModel> GetStudentsByPhoneNumber(string phoneNo, string clientId);
        Task<ResponseModel> GetStudentsByAddress(string address, string clientId);
        Task<ResponseModel> GetStudentsByName(string address, string clientId);
        Task<ResponseModel> GetStudentsByAcademicNo(string academicNo, string clientId);



    }
}
