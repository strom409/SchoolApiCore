using ClassTest.Repository;

namespace ClassTest.Services.Subject
{
    public interface ISubjectService
    {
        Task<ResponseModel> InsertNewSubject(SubjectDTO subname, string clientId);
        Task<ResponseModel> InsertNewOptionalSubject(SubjectDTO value, string clientId);
        Task<ResponseModel> InsertNewSubSubject(SubjectDTO value, string clientId);
        Task<ResponseModel> UpgradeSubjectAsync(SubjectDTO subject, string clientId);
        Task<ResponseModel> UpdateSubject(SubjectDTO value, string clientId);
        Task<ResponseModel> UpdateOptionalSubject(SubjectDTO value, string clientId);
        Task<ResponseModel> UpdateSubSubject(SubjectDTO value, string clientId);
        Task<ResponseModel> GetSubjectsByClassId(string? param, string clientId);
        Task<ResponseModel> GetOptionalSubjectsByClassId(string? param, string clientId);
        Task<ResponseModel> GetSubSubjectsBySubjectId(string? param, string clientId);
        Task<ResponseModel> DeleteSubject(string subjectId, string clientId);
        Task<ResponseModel> DeleteOptionalSubject(string optionalSubjectId, string clientId);
        Task<ResponseModel> DeleteSubSubject(string subSubjectId, string clientId);
    }
}
