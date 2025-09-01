using Examination_Management.Repository;

namespace Examination_Management.Services.MarksSheetSetting
{
    public interface IMarksSheetSettingService
    {
        Task<ResponseModel> SaveMarksSheetSetting(MarksSheetSettingDto dto, string clientId);
    }
}
