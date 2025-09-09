


using Azure;
using Student.Repository;

namespace Student.Services.Students
{
    public interface IStudentService
    {
        
        Task<ResponseModel> AddStudent(AddStudentRequestDTO request, string clientId);
        Task<ResponseModel> AddNewStudentWithRegNo(AddStudentRequestDTO request, string clientId);
        Task<ResponseModel>AddNewGPS(AddStudentRequestDTO request, string clientId);

        Task<ResponseModel> GetStudentsByClass(long classId, string clientId);
        Task<ResponseModel?> GetStudentByAdmissionNo(string admissionNo, string clientId);
        Task<ResponseModel> GetStudentsByName(string studentName, string clientId);
        Task<ResponseModel> GetStudentByStudentInfoId(long studentInfoId, string clientId);
        Task<ResponseModel?> GetStudentByPhone(string phoneNo, string clientId);
        Task<ResponseModel> GetStudentsByCurrentSession(string currentSession, string clientId);
        Task<ResponseModel> GetAllStudentsOnSectionID(string sectionId, string clientId);
        Task<ResponseModel> GetOnlyActiveStudentsOnClassID(long classId, string clientId);
        Task<ResponseModel> GetOnlyActiveStudentsOnSectionID(long sectionId, string clientId);
        Task<ResponseModel> GetMaxRollno(string sectionId, string clientId);
        Task<ResponseModel> GetAllDischargedStudentsOnSectionID(string sectionId, string clientId);
        Task<ResponseModel> TotalStudentsRollForDashBoard(string session, string clientId);
        Task<ResponseModel> ClassWisStudentsRollForDashBoard(string session, string clientId);
        Task<ResponseModel> TotalStudentsRollForDashBoardOnDate(string session, string clientId);
        Task<ResponseModel> SectionWisStudentsRollWithAttendanceForDashBoard(string  ClassID, string clientId);
       
        Task<ResponseModel> GetBoardNoWithDate(string classSectionId, string clientId);
        Task<ResponseModel> GetNextAdmissionNoAsync(string clientId);
        Task<ResponseModel> GetAllSessions(string clientId);
        Task<ResponseModel> GetStudentIdAsync(string StudentIdDto, string clientId);

        Task<ResponseModel> UpdateStudentAsync(UpdateStudentRequestDTO request, string clientId);

        Task<ResponseModel> UpdateParentDetail(UpdateStudentRequestDTO request, string clientId);
        Task<ResponseModel> UpdateAddressDetail(UpdateStudentRequestDTO request, string clientId);
        Task<ResponseModel> UpdatePersonalDetail(UpdateStudentRequestDTO request, string clientId);
        Task<ResponseModel> UpdateStudentRollNo(UpdateStudentRequestDTO request, string clientId);
        Task<ResponseModel> UpdateClassStudentRollNumbers(List<StudentRollNoUpdate> updates, string clientId);
        // Task<ResponseModel> UpdateClassStudentRollNumbers(UpdateStudentRequestDTO request, string clientId);
        Task<ResponseModel> UpdateBoardNo(UpdateStudentRequestDTO request, string clientId);
        Task<ResponseModel> UpdateDOB(UpdateStudentRequestDTO request, string clientId);
        Task<ResponseModel> UpdateSection(UpdateStudentRequestDTO request, string clientId);
        Task<ResponseModel> UpdateClass(UpdateStudentRequestDTO request, string clientId);

        Task<ResponseModel> DischargeStudent(UpdateStudentRequestDTO request, string clientId);
        Task<ResponseModel> DischargeStudentForIntValue(UpdateStudentRequestDTO request, string clientId);
        Task<ResponseModel> RejoinStudent(UpdateStudentRequestDTO request, string clientId);

        Task<ResponseModel> RejoinStudentForIntValue(UpdateStudentRequestDTO request, string clientId);

        Task<ResponseModel> UpdateStudentEducationAdmissionPrePrimaryEtc(UpdateStudentRequestDTO request, string clientId);

        Task<ResponseModel> UpdateStudentHeightWeightAdharNamePENEtcUDISE(UpdateStudentRequestDTO request, string clientId);

        Task<ResponseModel> UpdateStudentSessionAsync(StudentSessionUpdateRequest request, string clientId);

    }
}

