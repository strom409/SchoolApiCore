using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Timetable_Arrangement.Repository;
using Timetable_Arrangement.Services.TimeTableArrangements;
using Timetable_Arrangement.Services.TTAssignPeroids;

namespace Timetable_Arrangement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TimeTableArrangementsController : ControllerBase
    {
        private readonly ITimeTableArrangementsServices _services;

        public TimeTableArrangementsController(ITimeTableArrangementsServices services)
        {
            _services = services;
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
                        response = await _services.ArrangeTimetable(dto, clientId);
                        break;
                    default:
                        response.IsSuccess = false;
                        response.Message = "Invalid actionType for Add.";
                        break;
                }
            }
            catch (Exception ex)
            {
                Repository.Error.ErrorBLL.CreateErrorLog("TimeTableArrangementsController", "Add", ex.ToString());
                response.IsSuccess = false;
                response.Message = "Unexpected error.";
                response.Error = ex.Message;
            }

            return Ok(response);
        }

        [HttpGet("fetch")]
        public async Task<ActionResult<ResponseModel>> Get([FromQuery] int actionType, [FromQuery] string? param = null)
        {
            var response = new ResponseModel { IsSuccess = true, Status = 0, Message = "Issue at Controller Level!" };

            try
            {
                var clientId = Request.Headers["X-Client-Id"].FirstOrDefault() ?? "client1";

                switch (actionType)
                {
                    case 1: // Get timetable by date
                        response = await _services.GetTimeTableArrangementsByDate(param, clientId);
                        break;
                    case 2: // Get absent teacher timetable for today
                        response = await _services.GetTimeTableArrangementsOfAbsentTeacherToday(param, clientId);
                        break;

                    case 3: // Get all employees
                        response = await _services.GetEmployeeList(clientId);
                        break;

                    case 4: // Get employees not in timetable
                        if (string.IsNullOrEmpty(param))
                        {
                            response.IsSuccess = false;
                            response.Message = "Year parameter is required.";
                            break;
                        }
                        response = await _services.GetEmployeeListNotInTimeTable(param, clientId);
                        break;

                    case 5: // Get employees who are in timetable
                        if (string.IsNullOrEmpty(param))
                        {
                            response.IsSuccess = false;
                            response.Message = "Year parameter is required.";
                            break;
                        }
                        response = await _services.GetEmployeeListWhoAreInTimeTable(param, clientId);
                        break;
                    default:
                        response.IsSuccess = false;
                        response.Message = "Invalid actionType.";
                        break;
                }
            }
            catch (Exception ex)
            {
                Repository.Error.ErrorBLL.CreateErrorLog("TimeTableArrangementsController", "Get", ex.ToString());
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Unexpected error occurred.";
                response.Error = ex.Message;
            }

            return Ok(response);
        }


        [HttpDelete("delete")]
        public async Task<ActionResult<ResponseModel>> Delete([FromQuery] int actionType, [FromBody] JObject dto)
        {
            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0
            };

            try
            {
                var clientId = Request.Headers["X-Client-Id"].FirstOrDefault() ?? "client1";

                switch (actionType)
                {
                    case 1: // Delete Timetable
                        if (dto == null)
                            return BadRequest("TimeTableDelete object is required.");

                        var timetableDto = dto.ToObject<TimeTableDelete>();
                        if (timetableDto == null)
                            return BadRequest("Invalid TimeTableDelete object.");

                        response = await _services.DeleteTimetable(timetableDto, clientId);
                        break;

                    case 2: // Delete Arrangement Timetable
                        if (dto == null)
                            return BadRequest("TimeTable object is required.");

                        var arrangementDto = dto.ToObject<TimeTable>();
                        if (arrangementDto == null)
                            return BadRequest("Invalid TimeTable object.");

                        response = await _services.DeleteArrangementTimetable(arrangementDto, clientId);
                        break;

                    default:
                        response.IsSuccess = false;
                        response.Status = -1;
                        break;
                }
            }
            catch (Exception ex)
            {
                Repository.Error.ErrorBLL.CreateErrorLog("TimeTableArrangementsController", "Delete", ex.ToString());
                response.IsSuccess = false;
                response.Status = -1;
                response.Error = ex.Message;
            }

            return Ok(response);
        }


    }
}
