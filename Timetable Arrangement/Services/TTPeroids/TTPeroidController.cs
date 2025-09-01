using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Timetable_Arrangement.Repository;
using Timetable_Arrangement.Services.TTAssignPeroids;

namespace Timetable_Arrangement.Services.TTPeroids
{
    [Route("api/[controller]")]
    [ApiController]
    public class TTPeroidController : ControllerBase
    {
        private readonly ITTPeroidService _service;
        public TTPeroidController(ITTPeroidService service)
        {
            _service = service;
        }

        [HttpPost("add")]
        public async Task<ActionResult<ResponseModel>> Add([FromBody] TimeTable ttval, [FromQuery] int actionType)
        {
            var response = new ResponseModel { IsSuccess = true, Status = 0, Message = "Issue at Add level." };
            try
            {
                var clientId = Request.Headers["X-Client-Id"].FirstOrDefault() ?? "client1";

                switch (actionType)
                {
                    case 1: // Add new period
                        response = await _service.addTTperoidtime(ttval, clientId);
                        break;
                    default:
                        response.IsSuccess = false;
                        response.Message = "Invalid actionType for Add.";
                        break;
                }
            }
            catch (Exception ex)
            {
                Repository.Error.ErrorBLL.CreateErrorLog("TTPeroidController", "Add", ex.ToString());
                response.IsSuccess = false;
                response.Message = "Unexpected error.";
                response.Error = ex.Message;
            }

            return Ok(response);
        }

        [HttpPut("update")]
        public async Task<ActionResult<ResponseModel>> Update([FromBody] TimeTable ttval, [FromQuery] int actionType)
        {
            var response = new ResponseModel { IsSuccess = true, Status = 0, Message = "Issue at Update level." };
            try
            {
                var clientId = Request.Headers["X-Client-Id"].FirstOrDefault() ?? "client1";

                switch (actionType)
                {
                    case 1: // Update period
                        response = await _service.updateTTperoidtime(ttval, clientId);
                        break;
                    default:
                        response.IsSuccess = false;
                        response.Message = "Invalid actionType for Update.";
                        break;
                }
            }
            catch (Exception ex)
            {
                Repository.Error.ErrorBLL.CreateErrorLog("TTPeroidController", "Update", ex.ToString());
                response.IsSuccess = false;
                response.Message = "Unexpected error.";
                response.Error = ex.Message;
            }

            return Ok(response);
        }

        [HttpGet("get")]
        public async Task<ActionResult<ResponseModel>> Get([FromQuery] int actionType, [FromQuery] string? pid = null)
        {
            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "Issue at Get level."
            };

            try
            {
                var clientId = Request.Headers["X-Client-Id"].FirstOrDefault() ?? "client1";

                switch (actionType)
                {
                    case 1: // Get all timetable periods
                        response = await _service.Gettimetable(clientId);
                        break;
                    case 2: // Get period numbers
                        response = await _service.getPeroidNo(clientId);
                        break;
                    case 3: // Get timetable periods with durations
                        response = await _service.getTimeTablePeriodsWithDuration(clientId);
                        break;
                    case 4: // Get period list by pid
                        if (string.IsNullOrEmpty(pid))
                        {
                            response.IsSuccess = false;
                            response.Status = 0;
                            response.Message = "pid is required for this action.";
                        }
                        else
                        {
                            response = await _service.GetPeriodList(pid, clientId);
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
                Repository.Error.ErrorBLL.CreateErrorLog("TTPeroidController", "Get", ex.ToString());
                response.IsSuccess = false;
                response.Message = "Unexpected error.";
                response.Error = ex.Message;
            }

            return Ok(response);
        }

    }
}


