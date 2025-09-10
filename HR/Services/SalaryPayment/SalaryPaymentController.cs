using Azure;
using HR.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HR.Services.SalaryPayment
{
    [Route("api/[controller]")]
    [ApiController]
    public class SalaryPaymentController : ControllerBase
    {
        private readonly ISalaryPaymentService _salaryPaymentService;
        public SalaryPaymentController(ISalaryPaymentService salaryPaymentService)
        {
            _salaryPaymentService=salaryPaymentService;
        }


        [HttpPost("add-salary-payment")]
        [HttpPost("salary-payment")]
        public async Task<ActionResult<ResponseModel>> Post([FromQuery] int actionType, [FromBody] List<SalaryPayment> value)
        {
            #region Initialize Response
            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "Issue at Controller Level !"
            };
            #endregion

            try
            {
                var clientId = Request.Headers["X-Client-Id"].FirstOrDefault() ?? "client1";
                if (string.IsNullOrEmpty(clientId))
                    return BadRequest("ClientId header missing");

                switch (actionType)
                {
                    case 0:
                        response = await _salaryPaymentService.AddSalaryPayment(value, clientId);
                        break;

                    default:
                        response.IsSuccess = false;
                        response.Message = "Invalid actionType!";
                        break;
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error: " + ex.Message;
                Repository.Error.ErrorBLL.CreateErrorLog("SalaryPaymentController", "Post", ex.ToString());
            }

            return Ok(response);
        }



        [HttpGet("fetch-income-ledger-data")]
        public async Task<ActionResult<ResponseModel>> FetchIncomeLedgerData([FromQuery] int actionType, [FromQuery] string? param)
        {
            #region Get ClientId
            var clientId = Request.Headers["X-Client-Id"].FirstOrDefault() ?? "client1";
            if (string.IsNullOrEmpty(clientId))
                return BadRequest("ClientId header missing");
            #endregion

            #region Initialize Response
            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "Issue at Controller Level !"
            };
            #endregion

            try
            {
                switch (actionType)
                {
                    case 0:
                        response = await _salaryPaymentService.GetSalaryPaymentStatementOnMonthAndYear(param, clientId);
                        break;

                    // You can add more cases as needed for other types
                    default:
                        response.Message = "Invalid Type ID!";
                        break;
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error: " + ex.Message;
                Repository.Error.ErrorBLL.CreateErrorLog("SalaryPaymentController", "FetchIncomeLedgerData", ex.ToString());
            }

            return Ok(response);
        }
        [HttpPut("update-salary-data")]
        public async Task<ActionResult<ResponseModel>> UpdateSalaryData([FromQuery] int actionType, [FromBody] List<SalaryPayment> value)
        {
            #region Get ClientId
            var clientId = Request.Headers["X-Client-Id"].FirstOrDefault() ?? "client1";
            if (string.IsNullOrEmpty(clientId))
                return BadRequest("ClientId header missing");
            #endregion

            #region Initialize Response
            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "Issue at Controller Level !"
            };
            #endregion
            try
            {
                switch (actionType)
                {
                    case 0: // Delete Salary Payment
                        response = await _salaryPaymentService.DeleteSalaryPayment(value, clientId);
                        break;

                    default:
                        response.Message = "Invalid action type!";
                        break;
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error: " + ex.Message;
                Repository.Error.ErrorBLL.CreateErrorLog("SalaryPaymentController", "UpdateSalaryData", ex.ToString());
            }

            return Ok(response);
        }

    }
}
