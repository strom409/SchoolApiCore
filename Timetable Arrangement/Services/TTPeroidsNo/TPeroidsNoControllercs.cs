using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Timetable_Arrangement.Repository;

namespace Timetable_Arrangement.Services.TTPeroids
{
    [Route("api/[controller]")]
    [ApiController]
    public class TPeroidsNoControllercs : ControllerBase
    {
        private readonly ITTPeroidsNoService _service;
        public TPeroidsNoControllercs(ITTPeroidsNoService service)
        {
            _service = service;
        }

        [HttpGet("fetch")]
        public async Task<ActionResult<ResponseModel>> Fetch([FromQuery] int actionType, [FromQuery] long? pid = null)
        {
            var response = new ResponseModel { IsSuccess = true, Status = 0, Message = "Issue at Fetch level." };

            try
            {
                var clientId = Request.Headers["X-Client-Id"].FirstOrDefault() ?? "client1";

                switch (actionType)
                {
                  

                    case 1: // Get by PID
                        if (pid.HasValue)
                        {
                            response = await _service.PeroidName(pid.ToString(), clientId);
                        }
                        else
                        {
                            response.IsSuccess = false;
                            response.Message = "PID is required for actionType=2.";
                        }
                        break;

                    default:
                        response.IsSuccess = false;
                        response.Message = "Invalid actionType for Fetch.";
                        break;
                }
            }
            catch (Exception ex)
            {
                Repository.Error.ErrorBLL.CreateErrorLog("TTPeroidsNoController", "Fetch", ex.ToString());
                response.IsSuccess = false;
                response.Message = "Unexpected error.";
                response.Error = ex.Message;
            }

            return Ok(response);
        }

    }

}

