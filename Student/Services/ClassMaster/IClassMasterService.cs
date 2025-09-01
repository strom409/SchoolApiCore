


using Student.Repository;

namespace Student.Services.ClassMaster
{
    public interface IClassMasterService
    {
        Task<ResponseModel> GetEducationalDepartments(string clientId);
        Task<ResponseModel> GetSectionsByClassId(int classId,string clientId);
        Task<ResponseModel> GetClassesBySessionWithDepartment(string session, string clientId);
        Task<ResponseModel> AddClass(ClassDto classDto, string clientId);
        Task<ResponseModel> AddSection(SectionDto sectionDto, string clientId);
        Task<ResponseModel> UpgradeClassSubjectsSectionsAsync(UpgradeClassDto upgradeDto, string clientId);
        Task<ResponseModel> UpdateClass(ClassDto classDto, string clientId);
        Task<ResponseModel> UpdateSection(SectionDto sectionDto, string clientId);
        Task<ResponseModel> DeleteClass(int classId, string clientId);
        Task<ResponseModel> DeleteSection(int sectionId, string clientId);


    }
}
