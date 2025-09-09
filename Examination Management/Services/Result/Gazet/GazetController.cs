using Examination_Management.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Examination_Management.Services.Result.Gazet
{
    [Route("api/[controller]")]
    [ApiController]
    public class GazetController : ControllerBase
    {
        private readonly IGazetService _gazetService;
        public GazetController(IGazetService gazetService)
        {
            _gazetService = gazetService;
        }

        [HttpGet("fetch")]
        public async Task<ActionResult<ResponseModel>> Get([FromQuery] int actionType, [FromQuery] string? param = null)
        {
            var response = new ResponseModel { IsSuccess = true, Status = 0, Message = "Issue at Controller Level!" };

            try
            {
                var clientId = Request.Headers["X-Client-Id"].FirstOrDefault();
                if (string.IsNullOrEmpty(clientId))
                    return BadRequest("ClientId header missing");


                switch (actionType)
                {
                    case 1: // Get Gazet results by Class, Section, Session, Unit
                        response = await _gazetService.GetGazetResults(param, clientId);
                        break;

                    default:
                        response.IsSuccess = false;
                        response.Message = "Invalid actionType provided.";
                        break;
                }
            }
            catch (Exception ex)
            {
                Repository.Error.ErrorBLL.CreateErrorLog("GazetController", "Get", ex.ToString());
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "An unexpected error occurred.";
                response.Error = ex.Message;
            }

            return Ok(response);
        }

    }
}

