using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Timetable_Arrangement.Repository;
using Timetable_Arrangement.Services.TimeTableArrangements;

namespace Timetable_Arrangement.Services.TimeTableHistory
{
    [Route("api/[controller]")]
    [ApiController]
    public class TimeTableHistoryController : ControllerBase
    {
        private readonly ITimeTableHistoryServices _timeTableHistoryServices;
        public TimeTableHistoryController(ITimeTableHistoryServices timeTableHistoryServices)
        {
            _timeTableHistoryServices= timeTableHistoryServices;
        }

        [HttpPost("add")]
        public async Task<ActionResult<ResponseModel>> Add([FromBody] TimeTableDto dto, [FromQuery] int actionType)
        {
            var response = new ResponseModel { IsSuccess = true, Status = 0, Message = "Issue at Add level." };

            try
            {
                var clientId = Request.Headers["X-Client-Id"].FirstOrDefault() ?? "client1";

                switch (actionType)
                {
                    case 1:
                        response = await _timeTableHistoryServices.AddTimeTableTeacherHistory(dto, clientId);
                        break;

                    default:
                        response.IsSuccess = false;
                        response.Message = "Invalid actionType for Add.";
                        break;
                }
            }
            catch (Exception ex)
            {
                Repository.Error.ErrorBLL.CreateErrorLog("TimeTableHistoryController", "Add", ex.ToString());
                response.IsSuccess = false;
                response.Message = "Unexpected error.";
                response.Error = ex.Message;
            }

            return Ok(response);
        }

        [HttpGet("get")]
        public async Task<ActionResult<ResponseModel>> Get([FromQuery] int actionType, [FromQuery] string teacherId = null, [FromQuery] string dayId = null)
        {
            var response = new ResponseModel { IsSuccess = true, Status = 0, Message = "Issue at Get level." };

            try
            {
                var clientId = Request.Headers["X-Client-Id"].FirstOrDefault() ?? "client1";

                switch (actionType)
                {
                    case 1: // Get timetable history by teacher and day
                        if (!string.IsNullOrEmpty(teacherId) && !string.IsNullOrEmpty(dayId))
                        {
                            response = await _timeTableHistoryServices.GetTimeTableTeacherHistory(teacherId, dayId, clientId);
                        }
                        else
                        {
                            response.IsSuccess = false;
                            response.Message = "TeacherId and DayId are required for actionType=1.";
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
                Repository.Error.ErrorBLL.CreateErrorLog("TimeTableHistoryController", "Get", ex.ToString());
                response.IsSuccess = false;
                response.Message = "Unexpected error.";
                response.Error = ex.Message;
            }

            return Ok(response);
        }

    }
}
