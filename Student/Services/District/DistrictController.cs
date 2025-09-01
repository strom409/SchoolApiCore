using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Student.Repository;

namespace Student.Services.District
{
    [Route("api/[controller]")]
    [ApiController]
    public class DistrictController : ControllerBase
    {
        private readonly IDistrictService _districtService;
        public DistrictController(IDistrictService districtService)
        {
            _districtService=districtService;
        }

        [HttpGet("district-master")]
        public async Task<ActionResult<ResponseModel>> FetchDistrictInfo(int actionType, string? param)
        {
            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0
            };

            var clientId = Request.Headers["X-Client-Id"].FirstOrDefault();

            if (string.IsNullOrEmpty(clientId))
                return BadRequest("ClientId header missing");

            try
            {
                switch (actionType)
                {
                    case 0:
                        // Get all districts
                        return Ok(await _districtService.GetAllDistricts(clientId));

                    case 1:
                        // Get districts by state ID
                        if (!int.TryParse(param, out int stateId))
                            return BadRequest("Invalid StateID");

                        return Ok(await _districtService.GetDistrictsByStateId(stateId, clientId));
                    case 2:
                        return Ok(await _districtService.GetAllStates(clientId));

                    default:
                        response.IsSuccess = false;
                        response.Status = 0;
                        response.Message = "Invalid action type";
                        return BadRequest(response);
                }
            }
            catch (Exception ex)
            {
                Student.Repository.Error.ErrorBLL.CreateErrorLog("DistrictController", "FetchDistrictInfo", ex.ToString());

                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Exception occurred";
                response.Error = ex.Message;

                return StatusCode(500, response);
            }
        }
    }
}
