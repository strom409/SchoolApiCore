using FeeManagement.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace FeeManagement.Services.FeeDueRebate
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeeDueRebateController : ControllerBase
    {
        private readonly IFeeDueRebateService _feeDueRebateService;
        public FeeDueRebateController(IFeeDueRebateService feeDueRebateService)
        {
            _feeDueRebateService = feeDueRebateService;
        }
        [HttpPost("add-info")]
        public async Task<ActionResult<ResponseModel>> AddInfo([FromQuery] int actionType, [FromBody] object request)
        {
            var response = new ResponseModel { IsSuccess = true, Status = 0, Message = "Issue at Controller Level!" };

            try
            {
                var clientId = Request.Headers["X-Client-Id"].FirstOrDefault();
                if (string.IsNullOrEmpty(clientId))
                    return BadRequest("ClientId header missing");

                switch (actionType)
                {
                    case 0: // Add FeeDueRebate
                        var dto = JsonConvert.DeserializeObject<FeeDueRebateDTO>(request.ToString());
                        response = await _feeDueRebateService.AddFeeDueRebate(dto, clientId);
                        break;

                    default:
                        response.IsSuccess = false;
                        response.Message = "Invalid actionType.";
                        return Ok(response);
                }
            }
            catch (Exception ex)
            {
                Repository.Error.ErrorBLL.CreateErrorLog("FeeDueRebateController", "AddInfo", ex.ToString());
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "An unexpected error occurred.";
                response.Error = ex.Message;
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
                if (string.IsNullOrEmpty(clientId))
                    return BadRequest("ClientId header missing");

                switch (actionType)
                {
                    case 0: // Update FeeDueRebate
                        var feeDueRebateDto = JsonConvert.DeserializeObject<FeeDueRebateDTO>(dto.ToString());
                        if (feeDueRebateDto.RebateID <= 0)
                            return BadRequest(new ResponseModel { IsSuccess = false, Status = 0, Message = "Invalid RebateID for update." });

                        response = await _feeDueRebateService.UpdateFeeDueRebate(feeDueRebateDto, clientId);
                        break;

                    default:
                        response.Message = "Invalid actionType.";
                        break;
                }
            }
            catch (Exception ex)
            {
                Repository.Error.ErrorBLL.CreateErrorLog("FeeDueRebateController", "UpdateByAction", ex.ToString());
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "An error occurred during update.";
                response.Error = ex.Message;
            }

            return StatusCode(200, response);
        }

        [HttpGet("get-info")]
        public async Task<ActionResult<ResponseModel>> GetInfo([FromQuery] int actionType, [FromQuery] string param)
        {
            var response = new ResponseModel { IsSuccess = true, Status = 0, Message = "Invalid request." };

            try
            {
                var clientId = "client2";
              //  var clientId = Request.Headers["X-Client-Id"].FirstOrDefault();
                if (string.IsNullOrEmpty(clientId))
                    return BadRequest("ClientId header missing");

                switch (actionType)
                {
                    case 0: // Get by Student Name
                        if (string.IsNullOrEmpty(param))
                        {
                            response.Message = "StudentName parameter missing.";
                            break;
                        }
                        response = await _feeDueRebateService.GetFeeDueRebateByStudentName(param, clientId);
                        break;

                    case 1: // Get by AdmissionNo (param = "AdmissionNo,CurrentSession")
                        if (string.IsNullOrEmpty(param))
                        {
                            response.Message = "Parameter missing. Use AdmissionNo,CurrentSession.";
                            break;
                        }
                        response = await _feeDueRebateService.GetFeeDueRebateByAdmissionNo(param, clientId);
                        break;

                    case 2: // Get by ClassID
                        if (!long.TryParse(param, out long classId))
                        {
                            response.Message = "Invalid ClassID parameter.";
                            break;
                        }
                        response = await _feeDueRebateService.GetFeeDueRebateByClassId(classId, clientId);
                        break;

                    default:
                        response.Message = "Invalid actionType.";
                        break;
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error retrieving data.";
                response.Error = ex.Message;

                Repository.Error.ErrorBLL.CreateErrorLog("FeeDueRebateController", "GetInfo", ex.ToString());
            }

            return Ok(response);
        }


        [HttpDelete("delete")]
        public async Task<IActionResult> Delete([FromQuery] int actionType, [FromQuery] long id)
        {
            var response = new ResponseModel { IsSuccess = false, Status = 0, Message = "Invalid request." };

            try
            {
                var clientId = Request.Headers["X-Client-Id"].FirstOrDefault();
                if (string.IsNullOrEmpty(clientId))
                    return BadRequest("ClientId header missing");

                switch (actionType)
                {
                    case 0: // Delete FeeDueRebate (soft delete)
                        response = await _feeDueRebateService.DeleteFeeDueRebate(id, clientId);
                        break;

                    default:
                        response.IsSuccess = false;
                        response.Status = 0;
                        response.Message = "Invalid actionType.";
                        return BadRequest(response);
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error occurred during deletion.";
                response.Error = ex.Message;
                Repository.Error.ErrorBLL.CreateErrorLog("FeeDueRebateController", "Delete", ex.ToString());
            }

            return Ok(response);
        }
    }
}


