using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Timetable_Arrangement.Repository;

namespace Timetable_Arrangement.Services.TTDays
{
    [Route("api/[controller]")]
    [ApiController]
    public class TTDaysController : ControllerBase
    {
        private readonly ITTDaysService _ttsService;
        public TTDaysController(ITTDaysService ttsService)
        {
            _ttsService=ttsService;
        }

        [HttpGet("get")]
        public async Task<ActionResult<ResponseModel>> Get([FromQuery] int actionType, [FromQuery] string did = null, [FromQuery] string years = null)
        {
            var response = new ResponseModel { IsSuccess = true, Status = 0, Message = "Issue at Get level." };

            try
            {
                var clientId = Request.Headers["X-Client-Id"].FirstOrDefault() ?? "client1";

                switch (actionType)
                {
                    case 1: // Get day by ID
                        if (!string.IsNullOrEmpty(did))
                            response = await _ttsService.DaydName(did, clientId);
                        else
                        {
                            response.IsSuccess = false;
                            response.Message = "DID is required for actionType=1.";
                        }
                        break;

                    case 2: // Get all weekdays
                        response = await _ttsService.getweekdays(clientId);
                        break;

                    case 3: // Get whole timetable by year
                        if (!string.IsNullOrEmpty(years))
                            response = await _ttsService.GetwholetimetableProc(years, clientId);
                        else
                        {
                            response.IsSuccess = false;
                            response.Message = "Years is required for actionType=3.";
                        }
                        break;

                    default:
                        response.IsSuccess = false;
                        response.Message = "Invalid actionType for Get.";
                        break;
                }
            }
            catch (Exception ex)
            {
                Repository.Error.ErrorBLL.CreateErrorLog("TTDaysController", "Get", ex.ToString());
                response.IsSuccess = false;
                response.Message = "Unexpected error.";
                response.Error = ex.Message;
            }

            return Ok(response);
        }

       
    }
}
