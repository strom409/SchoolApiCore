using HR.Repository;
using HR.Services.Salary;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HR.Services.GrossPay
{
    [Route("api/[controller]")]
    [ApiController]
    public class GrossPayController : ControllerBase
    {
        private readonly IGrossPayService _grossPayService;
        public GrossPayController(IGrossPayService grossPayService)
        {
            _grossPayService = grossPayService;
        }

        [HttpPost("update-salary-grosspay")]
        public async Task<ActionResult<ResponseModel>> Post([FromQuery] int actionType, [FromBody] object data)
        {
            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "Issue at Controller Level !"
            };

            var clientId = Request.Headers["X-Client-Id"].FirstOrDefault();
            if (string.IsNullOrEmpty(clientId))
                return BadRequest("ClientId header missing");

            try
            {
                switch (actionType)
                {
                    case 0:
                        // Assuming data is List<GrossPaySalary> here
                        var grossPayList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<GrossPaySalary>>(data.ToString());
                        if (grossPayList == null || !grossPayList.Any())
                        {
                            response.Message = "Salary list is empty or invalid";
                            break;
                        }
                        response = await _grossPayService.UpdateSalaryDetailsOnGrossPay(grossPayList
                            
                            
                            );
                        break;

                    default:
                        response.Message = "Invalid actionType!";
                        break;
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error: " + ex.Message;
                Repository.Error.ErrorBLL.CreateErrorLog("GrossPayController", "Post", ex.ToString());
            }

            return Ok(response);
        }


        [HttpGet("get-salary-data")]
        public async Task<ActionResult<ResponseModel>> Get([FromQuery] int actionType, [FromQuery] string? employeeCode, [FromQuery] string? year, [FromQuery] string? edid)
        {
            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "Issue at Controller Level!"
            };

            var clientId = Request.Headers["X-Client-Id"].FirstOrDefault();
            if (string.IsNullOrEmpty(clientId))
                return BadRequest("ClientId header missing");

            try
            {
                switch (actionType)
                {
                    case 0:
                        response = await _grossPayService.CalculateSalaryDetailsOnGrossPay(employeeCode, clientId);
                        break;

                    case 1:
                        response = await _grossPayService.GetSalaryDetailsOnGrossPay(year, clientId);
                        break;

                    case 2:
                        response = await _grossPayService.GetSalaryDetailsOnGrossPayEDID(edid, clientId);
                        break;

                    default:
                        response.IsSuccess = false;
                        response.Status = -1;
                        response.Message = "Invalid actionType!";
                        break;
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error: " + ex.Message;
                Repository.Error.ErrorBLL.CreateErrorLog("PayRollController", "Get", ex.ToString());
            }

            return Ok(response);
        }

        [HttpPut("update-salary")]
        public async Task<ActionResult<ResponseModel>> Put([FromQuery] int actionType, [FromBody] List<GrossPaySalary> value)
        {
            var response = new ResponseModel
            {
                IsSuccess = false,
                Status = 0,
                Message = "Invalid Request!"
            };

            // Get clientId from headers
            var clientId = Request.Headers["X-Client-Id"].FirstOrDefault();
            if (string.IsNullOrEmpty(clientId))
            {
                response.Message = "ClientId header missing";
                return BadRequest(response);
            }

            try
            {
                switch (actionType)
                {
                    case 0: // Update Salary Calculated on GrossPay
                        if (value == null || !value.Any())
                        {
                            response.Message = "Salary list is empty or invalid";
                            return BadRequest(response);
                        }
                        response = await _grossPayService.UpdateSalaryDetailsOnGrossPay(value, clientId);
                        break;

                    default:
                        response.Message = "Invalid actionType!";
                        break;
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error: " + ex.Message;
                Repository.Error.ErrorBLL.CreateErrorLog("GrossPayController", "Put", ex.ToString());
            }

            return Ok(response);
        }
    }
}

