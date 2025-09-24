using FeeManagement.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FeeManagement.Services.FeeHead
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeeHeadController : ControllerBase
    {
        private readonly IFeeHeadService _feeHeadService;
        public FeeHeadController(IFeeHeadService feeHeadService)
        {
            _feeHeadService=feeHeadService; 
        }

        [HttpPost("add-info")]
        public async Task<ActionResult<ResponseModel>> AddInfo([FromQuery] int actionType, [FromBody] object request)
        {
            var response = new ResponseModel { IsSuccess = true, Status = 0, Message = "Issue at Controller Level!" };

            try
            {
                  var clientId = Request.Headers["X-Client-Id"].FirstOrDefault();
               // var clientId = "client1";

                if (string.IsNullOrEmpty(clientId))
                    return BadRequest("ClientId header missing");

                switch (actionType)
                {
                    case 0: // Add FeeHead
                        var feeHeadDto = JsonConvert.DeserializeObject<FeeHeadDto>(request.ToString());
                        response = await _feeHeadService.AddFeeHead(feeHeadDto, clientId);
                        break;

                    default:
                        response.IsSuccess = false;
                        response.Message = "Invalid actionType.";
                        return Ok(response);
                }
            }
            catch (Exception ex)
            {
               Repository.Error.ErrorBLL.CreateErrorLog("FeeHeadController", "AddInfo", ex.ToString());
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "An unexpected error occurred.";
                response.Error = ex.Message;
            }

            return Ok(response);
        }

        [HttpGet("fetch")]
        public async Task<ActionResult<ResponseModel>> Fetch([FromQuery] int actionType, [FromQuery] string? param)
        {
            var response = new ResponseModel { IsSuccess = true, Status = 0 };

             var clientId = Request.Headers["X-Client-Id"].FirstOrDefault();
           // var clientId = "client1";
            if (string.IsNullOrEmpty(clientId))
                return BadRequest("ClientId header missing");

            try
            {
                switch (actionType)
                {
                    case 0: // Get All FeeHeads
                        response = await _feeHeadService.GetAllFeeHeads(clientId);
                        break;

                    case 1: // Get FeeHead By ID
                        if (!long.TryParse(param, out long fHID))
                            return BadRequest(response);

                        response = await _feeHeadService.GetFeeHeadById(fHID, clientId);
                        break;
                    case 2: // Get FeeHeads By FHType
                        if (!int.TryParse(param, out int fHType))
                        {
                            response.Message = "Invalid FHType parameter.";
                            return BadRequest(response);
                        }

                        response = await _feeHeadService.GetFeeHeadsByType(fHType, clientId);
                        break;

                    default:
                        return BadRequest(response);
                }
            }
            catch (Exception ex)
            {
               Repository.Error.ErrorBLL.CreateErrorLog("FeeHeadController", "Fetch", ex.ToString());
                response.IsSuccess = false;
                response.Status = -1;
                response.Error = ex.Message;
                return StatusCode(500, response);
            }

            return Ok(response);
        }

        [HttpPut("update-by-action")]
        public async Task<ActionResult<ResponseModel>> Update([FromQuery] int actionType, [FromBody] object dto)
        {
            var response = new ResponseModel { IsSuccess = false, Status = 0, Message = "Invalid request." };

            try
            {
                  var clientId = Request.Headers["X-Client-Id"].FirstOrDefault();
               // var clientId = "client1";
                if (string.IsNullOrEmpty(clientId))
                    return BadRequest("ClientId header missing");

                switch (actionType)
                {
                    case 0: // Update FeeHead
                        var feeHeadDto = JsonConvert.DeserializeObject<FeeHeadDto>(dto.ToString());
                        if (feeHeadDto.FHID <= 0)
                            return BadRequest(new ResponseModel { IsSuccess = false, Status = 0, Message = "Invalid FHID for update." });

                        response = await _feeHeadService.UpdateFeeHead(feeHeadDto, clientId);
                        break;

                    default:
                        response.Message = "Invalid actionType.";
                        break;
                }
            }
            catch (Exception ex)
            {
                Repository.Error.ErrorBLL.CreateErrorLog("FeeHeadController", "UpdateByAction", ex.ToString());
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "An error occurred during update.";
                response.Error = ex.Message;
            }

            return StatusCode(200, response);
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> Delete([FromQuery] int actionType, [FromQuery] long id)
        {
            var response = new ResponseModel { IsSuccess = false, Status = 0, Message = "Invalid request." };

            try
            {
                var clientId = "client2";
                // var clientId = Request.Headers["X-Client-Id"].FirstOrDefault();

                if (string.IsNullOrEmpty(clientId))
                    return BadRequest("ClientId header missing.");

                switch (actionType)
                {
                    case 0: // Delete FeeHead (soft delete)
                        response = await _feeHeadService.DeleteFeeHead(id, clientId, "System");
                        break;

                    default:
                        response.IsSuccess = false;
                        response.Status = 0;
                        response.Message = "Invalid action type.";
                        return BadRequest(response);
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error occurred during deletion.";
                response.Error = ex.Message;

                Repository.Error.ErrorBLL.CreateErrorLog("FeeHeadController", "Delete", ex.ToString());
            }

            return Ok(response);
        }

    }
}

