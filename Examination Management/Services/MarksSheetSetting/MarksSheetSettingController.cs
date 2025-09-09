using Examination_Management.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Examination_Management.Services.MarksSheetSetting
{
    [Route("api/[controller]")]
    [ApiController]
    public class MarksSheetSettingController : ControllerBase
    { 
        private readonly IMarksSheetSettingService _marksSheetSettingService;
        public MarksSheetSettingController(IMarksSheetSettingService marksSheetSettingService)
        {
            _marksSheetSettingService= marksSheetSettingService;
        }

        [HttpPut("put")]
        public async Task<ActionResult<ResponseModel>> Put([FromQuery] int actionType, [FromBody] object request)
        {
            var response = new ResponseModel { IsSuccess = true, Status = 0, Message = "Issue at Controller Level!" };

            try
            {
                var clientId = Request.Headers["X-Client-Id"].FirstOrDefault();
                if (string.IsNullOrEmpty(clientId))
                    return BadRequest("ClientId header missing");

                switch (actionType)
                {
                    case 1: // Save or Update MarksSheetSetting
                        var dto = JsonConvert.DeserializeObject<MarksSheetSettingDto>(request.ToString());
                        response = await _marksSheetSettingService.SaveMarksSheetSetting(dto, clientId);
                        break;

                  

                    default:
                        response.IsSuccess = false;
                        response.Message = "Invalid actionType provided.";
                        break;
                }
            }
            catch (Exception ex)
            {
                Repository.Error.ErrorBLL.CreateErrorLog("MarksSheetSettingController", "Post", ex.ToString());
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "An unexpected error occurred.";
                response.Error = ex.Message;
            }

            return Ok(response);
        }
    }
}

