using FeeManagement.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace FeeManagement.Services.FeeDue
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeeDueController : ControllerBase
    {
        private readonly IFeeDueService _feeDueService;
        public FeeDueController(IFeeDueService feeDueService)
        {
            _feeDueService=feeDueService;
        }

        [HttpPost("add")]
        public async Task<ActionResult<ResponseModel>> Add([FromQuery] int actionType, [FromBody] object request)
        {
            var response = new ResponseModel { IsSuccess = true, Status = 0, Message = "Issue at Controller Level!" };

            try
            {
                var clientId = Request.Headers["X-Client-Id"].FirstOrDefault();
                if (string.IsNullOrEmpty(clientId))
                    return BadRequest("ClientId header missing");

                switch (actionType)
                {
                    case 0: // Add single FeeDue
                        var feeDto = JsonConvert.DeserializeObject<FeeDueDTO>(request.ToString());
                        response = await _feeDueService.AddFeeDue(feeDto, clientId);
                        break;

                   

                    // You can add more cases here for other add types
                    default:
                        response.IsSuccess = false;
                        response.Message = "Invalid actionType.";
                        return BadRequest(response);
                }
            }
            catch (Exception ex)
            {
                Repository.Error.ErrorBLL.CreateErrorLog("FeeDueController", "Add", ex.ToString());
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error adding FeeDue.";
                response.Error = ex.Message;
            }

            return Ok(response);
        }
        [HttpGet("fetch")]
        public async Task<ActionResult<ResponseModel>> Fetch([FromQuery] int actionType, [FromQuery] string? param)
        {
            var response = new ResponseModel { IsSuccess = true, Status = 0, Message = "Issue at Controller Level!" };

            try
            {
                var clientId = Request.Headers["X-Client-Id"].FirstOrDefault();
                if (string.IsNullOrEmpty(clientId))
                    return BadRequest("ClientId header missing");

                switch (actionType)
                {
                    case 0: // Get FeeDue by Student Name
                        if (string.IsNullOrEmpty(param))
                        {
                            response.IsSuccess = false;
                            response.Message = "StudentName parameter missing.";
                            return BadRequest(response);
                        }
                        response = await _feeDueService.GetFeeDueByStudentName(param, clientId);
                        break;

                    case 1: // Get FeeDue by Admission No
                        if (string.IsNullOrEmpty(param))
                        {
                            response.IsSuccess = false;
                            response.Message = "AdmissionNo parameter missing.";
                            return BadRequest(response);
                        }
                        response = await _feeDueService.GetFeeDueByAdmissionNo(param, clientId);
                        break;

                    case 2: // Get FeeDue by ClassID
                        if (!long.TryParse(param, out long classId))
                        {
                            response.IsSuccess = false;
                            response.Message = "Invalid ClassID.";
                            return BadRequest(response);
                        }
                        response = await _feeDueService.GetFeeDueByClassId(classId, clientId);
                        break;

                    default:
                        response.IsSuccess = false;
                        response.Message = "Invalid actionType.";
                        return BadRequest(response);
                }
            }
            catch (Exception ex)
            {
                Repository.Error.ErrorBLL.CreateErrorLog("FeeDueController", "Fetch", ex.ToString());
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error fetching FeeDue.";
                response.Error = ex.Message;
            }

            return Ok(response);
        }

        [HttpDelete("delete")]
        public async Task<ActionResult<ResponseModel>> Delete([FromQuery] int actionType, [FromQuery] long feeDueID)
        {
            var response = new ResponseModel { IsSuccess = true, Status = 0, Message = "Issue at Controller Level!" };

            try
            {
                var clientId = Request.Headers["X-Client-Id"].FirstOrDefault();
                if (string.IsNullOrEmpty(clientId))
                    return BadRequest("ClientId header missing");

                switch (actionType)
                {
                    case 0: // Delete FeeDue by FeeDueID
                        if (feeDueID <= 0)
                        {
                            response.IsSuccess = false;
                            response.Message = "Invalid FeeDueID.";
                            return BadRequest(response);
                        }
                        response = await _feeDueService.DeleteFeeDue(feeDueID, clientId);
                        break;

                    default:
                        response.IsSuccess = false;
                        response.Message = "Invalid actionType.";
                        return BadRequest(response);
                }
            }
            catch (Exception ex)
            {
                Repository.Error.ErrorBLL.CreateErrorLog("FeeDueController", "Delete", ex.ToString());
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error deleting FeeDue.";
                response.Error = ex.Message;
            }

            return Ok(response);
        }


    }
}
