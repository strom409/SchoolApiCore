using HR.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace HR.Services.Salary
{
    [Route("api/[controller]")]
    [ApiController]
    public class SalaryController : ControllerBase
    {
         private readonly ISalaryService _salaryService;
        public SalaryController(ISalaryService salaryService)
        {
            _salaryService=salaryService;
        }


        [HttpPut("salary")]
        public async Task<IActionResult> SalaryActions([FromQuery] int actionType, [FromBody] SalaryData request)
        {
            #region Initialize Response
            ResponseModel response = new ResponseModel
            {
                IsSuccess = false,
                Status = -1,
                Message = "Invalid ActionType"
            };
            #endregion

            #region Get ClientId from Header
            var clientId = Request.Headers["X-Client-Id"].FirstOrDefault() ?? "client1";
            if (string.IsNullOrEmpty(clientId))
                return BadRequest("ClientId header missing");
            #endregion

            try
            {
                #region Switch ActionType
                switch (actionType)
                {
                    case 0:
                        #region Salary Release on Departments
                        if (request.Salary.Count > 0)
                            response = await _salaryService.SalaryReleaseOnDepartments(request.Salary[0], clientId);
                        break;
                    #endregion

                    case 1:
                        #region Salary Release on Employee Code
                        if (request.Salary.Count > 0)
                            response = await _salaryService.SalaryReleaseOnEmployeeCode(request.Salary[0], clientId);
                        break;
                    #endregion

                    case 2:
                        #region Update Salary Details
                        if (request.EMD.Count > 0)
                            response = await _salaryService.UpdateSalaryDetails(request.EMD[0], clientId);
                        break;
                    #endregion

                    case 3:
                        #region Get Employee Salary To Edit
                        response = await _salaryService.GetEmployeeSalaryToEdit(request.ECode, clientId);
                        break;
                    #endregion

                    case 4:
                        #region Update Salary Details On Field
                        response = await _salaryService.UpdateSalaryDetailsOnField(request.EMD, clientId);
                        break;
                    #endregion

                    case 5:
                        #region Delete Salary On Employee Code
                        var salaryJson = JsonConvert.SerializeObject(request.Sal);
                      //  response = await _salaryService.AddNewLoan(salaryJson, clientId);
                        response = await _salaryService.DeleteSalaryOnEmployeeCode(salaryJson, clientId);
                        break;
                    #endregion

                    case 6:
                        #region Delete Salary On Departments
                        response = await _salaryService.DeleteSalaryOnDepartments(request.Salary, clientId);
                        break;
                    #endregion

                    case 7:
                        #region Get Demo Salary Statement
                        if (request.Salary.Count > 0)
                            response = await _salaryService.GetDemoSalaryOnDepartments(request.Salary[0], clientId);
                        break;
                    #endregion

                    case 8:
                        #region Add New Loan
                        var salary = JsonConvert.SerializeObject(request.Sal);
                        response = await _salaryService.AddNewLoan(salary, clientId);
                   //     response = await _salaryService.AddNewLoan(request.Sal, clientId);
                        break;
                    #endregion

                    default:
                        #region Unknown ActionType
                        response.Message = "Unknown ActionType!";
                        break;
                        #endregion
                }
                #endregion
            }
            catch (Exception ex)
            {
                #region Exception Handling
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error: " + ex.Message;
                #endregion
            }

            #region Return Response
            return Ok(response);
            #endregion
        }




        [HttpGet("fetch-salary-data")]
        public async Task<ActionResult<ResponseModel>> FetchSalaryData([FromQuery] int actionType, [FromQuery] string? param)
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
                Message = "Invalid  Request!",
                ResponseData = null
            };
            #endregion

            try
            {
                switch (actionType)
                {
                    case 0:
                        response = await _salaryService.GetEmployeeSalaryToEditOnEDID(param, clientId);
                        break;

                    case 1:
                        response = await _salaryService.GetEmployeeSalaryToEditOnECode(param, clientId);
                        break;

                    case 2:
                        response = await _salaryService.GetEmployeeSalaryToEditOnFieldName(param, clientId);
                        break;

                    case 3:
                        response = await _salaryService.GetSalaryDataOnMonthFromSalaryOnDeparts(param, clientId);
                        break;

                    case 4:
                        response = await _salaryService.GetCalculatedGrossNetEtc(param, clientId);
                        break;

                    case 5:
                        response = await _salaryService.GetCalculatedGrossNetEtcOnEDID(param, clientId);
                        break;

                    case 6:
                        response = await _salaryService.GetSalaryDataOnYearFromSalaryOnECode(param, clientId);
                        break;

                    case 7:
                        response = await _salaryService.GetLoanDefaultList(clientId);
                        break;

                    case 8:
                        response = await _salaryService.SalaryPaymentAccountStatementOnEcodeAndDates(param, clientId);
                        break;

                    case 9:
                        response = await _salaryService.GetAvailableNetSalaryOnMonthFromSalaryAndSalaryPaymentOnDeparts(param, clientId);
                        break;

                    case 10:
                        response = await _salaryService.GetBankSalarySlipOnMonthFromSalaryAndSalaryPaymentOnDeparts(param, clientId);
                        break;

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
                Repository.Error.ErrorBLL.CreateErrorLog("SalaryController", "FetchSalaryData", ex.ToString());
            }

            return Ok(response);
        }
    }
}
