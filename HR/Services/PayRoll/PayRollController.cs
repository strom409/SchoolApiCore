using Azure.Core;
using HR.Repository;
using HR.Services.Salary;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace HR.Services.PayRoll
{
    [Route("api/[controller]")]
    [ApiController]
    public class PayRollController : ControllerBase
    {
        private readonly IPayRollService _payRollService;

        public PayRollController(IPayRollService payRollService)
        {
            _payRollService = payRollService;
        }

        [HttpPost("process-loan-payment")]
        public async Task<ActionResult<ResponseModel>> Post([FromQuery] int actionType, [FromBody] LoanPayment SR)
        {
            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "Issue at Controller Level !"
            };
            #region Get ClientId
            var clientId = Request.Headers["X-Client-Id"].FirstOrDefault();
            if (string.IsNullOrEmpty(clientId))
                return BadRequest("ClientId header missing");
            #endregion

            try
            {
                switch (actionType)
                {
                    case 0:
                        response = await _payRollService.PayLoan(SR, clientId);
                        break;

                    case 1:
                        response = await _payRollService.PayCPFLoan(SR, clientId);
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
                Repository.Error.ErrorBLL.CreateErrorLog("PayRollController", "Post", ex.ToString());
            }

            return Ok(response);
        }



        [HttpPut("update-salary-data")]
        public async Task<ActionResult<ResponseModel>> Put([FromQuery] int actionType, [FromBody] JObject data)
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
                        // This method takes only clientId, no model required
                        response = await _payRollService.SetLeavesTaken(clientId);
                        break;

                    case 1:
                        // Deserialize data to SalaryModel for IssueNewLoan
                        var newLoanModel = data.ToObject<SalaryModel>();
                        if (newLoanModel == null)
                            return BadRequest("Invalid SalaryModel data for IssueNewLoan");
                        response = await _payRollService.IssueNewLoan(newLoanModel, clientId);
                        break;

                    case 2:
                        // Deserialize data to SalaryModel for IssueCPFLoan
                        var cpfLoanModel = data.ToObject<SalaryModel>();
                        if (cpfLoanModel == null)
                            return BadRequest("Invalid SalaryModel data for IssueCPFLoan");
                        response = await _payRollService.IssueCPFLoan(cpfLoanModel, clientId);
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
                response.Message = "Error: " + ex.ToString();
                Repository.Error.ErrorBLL.CreateErrorLog("PayRollController", "Put", ex.ToString());
            }

            return Ok(response);
        }




        [HttpGet("fetch-loan-details")]
        public async Task<ActionResult<ResponseModel>> FetchLoanDetails([FromQuery] int actionType, [FromQuery] string? param)
        {

            #region Initialize Response
            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "Issue at Controller Level !"
            };
            #endregion
            #region Get ClientId
            var clientId = Request.Headers["X-Client-Id"].FirstOrDefault() ?? "client1";
            if (string.IsNullOrEmpty(clientId))
                return BadRequest("ClientId header missing");
            #endregion
            try
            {
                switch (actionType)
                {
                    case 0:
                        if (!string.IsNullOrWhiteSpace(param) && long.TryParse(param, out long employeeId))
                        {
                            response = await _payRollService.GetLoanDetails(employeeId, clientId);
                        }
                        break;

                    case 1:
                        if (!string.IsNullOrWhiteSpace(param) && long.TryParse(param, out long employeeId1))
                        {
                            response = await _payRollService.GetCPFLoanDetails(employeeId1, clientId);
                        }
                        else
                        {
                            response.Message = "Invalid Employee ID for CPF Loan Details.";
                        }
                        break;

                    case 2:
                        response = await _payRollService.GetCPFLoanDefaultList(clientId);
                        break;

                    case 3:
                        response = await _payRollService.GetLoanDefaultList(clientId);
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
                Repository.Error.ErrorBLL.CreateErrorLog("PayRollController", "FetchLoanDetails", ex.ToString());
            }

            return Ok(response);
        }
    }
}
