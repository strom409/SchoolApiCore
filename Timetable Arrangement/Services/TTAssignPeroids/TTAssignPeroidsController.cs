using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Timetable_Arrangement.Repository;

namespace Timetable_Arrangement.Services.TTAssignPeroids
{
    [Route("api/[controller]")]
    [ApiController]
    public class TTAssignPeroidsController : ControllerBase
    {
        private readonly ITTAssignPeroidsService _service;
        public TTAssignPeroidsController(ITTAssignPeroidsService service)
        {
            _service=service;
        }


        [HttpPost("save")]
        public async Task<ActionResult<ResponseModel>> Save([FromBody] TimeTable dto, [FromQuery] int actionType)
        {
            var response = new ResponseModel { IsSuccess = true, Status = 0, Message = "Issue at controller level." };
            try
            {
                var clientId = Request.Headers["X-Client-Id"].FirstOrDefault() ?? "client1";

                switch (actionType)
                {
                    case 1: // Assign timetable
                        response = await _service.AssignTimetable(dto, clientId);
                        break;
                    case 2: // Swap timetable
                        response = await _service.SwapTimeTable(dto, clientId);
                        break;
                    default:
                        response.IsSuccess = false;
                        response.Message = "Invalid actionType.";
                        break;
                }
            }
            catch (Exception ex)
            {
                Repository.Error.ErrorBLL.CreateErrorLog("TTAssignPeroidsController", "Save", ex.ToString());
                response.IsSuccess = false;
                response.Message = "Unexpected error.";
                response.Error = ex.Message;
            }
            return Ok(response);
        }

        [HttpPut("update")]
        public async Task<ActionResult<ResponseModel>> Update([FromBody] TimeTable dto, [FromQuery] int actionType)
        {
            var response = new ResponseModel { IsSuccess = true, Status = 0, Message = "Issue at controller level." };
            try
            {
                var clientId = Request.Headers["X-Client-Id"].FirstOrDefault() ?? "client1";

                switch (actionType)
                {
                    case 1: // Update assigned timetable
                        response = await _service.UpdateAssignedTimetable(dto, clientId);
                        break;
                    default:
                        response.IsSuccess = false;
                        response.Message = "Invalid actionType.";
                        break;
                }
            }
            catch (Exception ex)
            {
                Repository.Error.ErrorBLL.CreateErrorLog("TTAssignPeroidsController", "Update", ex.ToString());
                response.IsSuccess = false;
                response.Message = "Unexpected error.";
                response.Error = ex.Message;
            }
            return Ok(response);
        }

        [HttpGet("fetch")]
        public async Task<ActionResult<ResponseModel>> Fetch([FromQuery] int actionType, [FromQuery] string teacherId = null, [FromQuery] string session = null)
        {
            var response = new ResponseModel { IsSuccess = true, Status = 0, Message = "Issue at controller level." };
            try
            {
                var clientId = Request.Headers["X-Client-Id"].FirstOrDefault() ?? "client1";

                switch (actionType)
                {
                    case 1: // Get all timetable
                        response = await _service.Getwholetimetable(clientId);
                        break;
                    case 2: // Get timetable by teacherId
                        if (!string.IsNullOrEmpty(teacherId))
                            response = await _service.GetassignedTT(teacherId, clientId);
                        else
                        {
                            response.IsSuccess = false;
                            response.Message = "teacherId is required for actionType=2.";
                        }
                        break;
                    case 3: // Get timetable for teacher with details
                        if (!string.IsNullOrEmpty(teacherId))
                            response = await _service.GetTeacherTimeTable(teacherId, clientId);
                        else
                        {
                            response.IsSuccess = false;
                            response.Message = "teacherId is required for actionType=3.";
                        }
                        break;
                    case 4: // Get timetable with current session
                        if (!string.IsNullOrEmpty(session))
                            response = await _service.GetTimeTableWithCurrentSession(session, clientId);
                        else
                        {
                            response.IsSuccess = false;
                            response.Message = "session is required for actionType=4.";
                        }
                        break;
                    default:
                        response.IsSuccess = false;
                        response.Message = "Invalid actionType.";
                        break;
                }
            }
            catch (Exception ex)
            {
                Repository.Error.ErrorBLL.CreateErrorLog("TTAssignPeroidsController", "Fetch", ex.ToString());
                response.IsSuccess = false;
                response.Message = "Unexpected error.";
                response.Error = ex.Message;
            }
            return Ok(response);
        }
    }
}

