using HR.Repository;

namespace HR.Services.Qualifications
{
    public interface IQualificationsService
    {
        Task<ResponseModel> GetQualifications(string clientId);

        Task<ResponseModel> GetQualificationById(string qualificationId, string clientId);

        Task<ResponseModel> AddQualification(QualificationModel qualification, string clientId);

        Task<ResponseModel> UpdateQualification(QualificationModel qualification, string clientId);

        Task<ResponseModel> DeleteQualification(string qualificationId, string clientId);

    }
}
