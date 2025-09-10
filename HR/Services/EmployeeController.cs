using HR.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HR.Services
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;
        public EmployeeController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }


        [HttpPost("manage")]
        public async Task<ActionResult<ResponseModel>> ManageEmployee(
    [FromQuery] int actionType,
    [FromBody] EmployeeDetail emp)
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
            var clientId = Request.Headers["X-Client-Id"].FirstOrDefault();
            if (string.IsNullOrEmpty(clientId))
                return BadRequest("ClientId header missing");
            #endregion

            try
            {
                switch (actionType)
                {
                    case 0: // Add New Employee
                        response = await _employeeService.AddNewEmployee(emp, clientId);
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
                #region Handle Exception
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error: " + ex.Message;
                Repository.Error.ErrorBLL.CreateErrorLog("EmployeeController", "ManageEmployee", ex.ToString());
                #endregion
            }

            return Ok(response);
        }


        [HttpPut("manage")]
        public async Task<ActionResult<ResponseModel>> ManageEmployeeUpdates(
    [FromQuery] int actionType,
    [FromBody] EmployeeDetail value)
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
            var clientId = Request.Headers["X-Client-Id"].FirstOrDefault();
            if (string.IsNullOrEmpty(clientId))
                return BadRequest("ClientId header missing");
            #endregion

            try
            {
                #region Switch Case for Operations
                switch (actionType)
                {
                    case 0: // Update Employee Details
                        response = await _employeeService.UpdateEmployee(value, clientId);
                        break;

                    case 1: // Update Employees Table On Field Name
                        response = await _employeeService.UpdateMultipleEmployee(value, clientId);
                        break;

                    case 2: // Update Employees Monthly Attendance
                        response = await _employeeService.UpdateEmployeeMonthlyAttendance(value, clientId);
                        break;

                    case 3: // Withdraw EMPLOYEE
                        response = await _employeeService.WithdrawEmployee(value, clientId);
                        break;

                    case 4: // Rejoin EMPLOYEE
                        response = await _employeeService.RejoinEmployee(value, clientId);
                        break;

                    case 5: // Update EmployeeDetails Table On Field Name
                        response = await _employeeService.UpdateEmployeeDetailField(value, clientId);
                        break;

                    default:
                        response.Status = -1;
                        response.Message = "Invalid actionType!";
                        break;
                }
                #endregion
            }
            catch (Exception ex)
            {
                #region Handle Exception
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error: " + ex.Message;
                Repository.Error.ErrorBLL.CreateErrorLog("EmployeeController", "ManageEmployeeUpdates", ex.ToString());
                #endregion
            }

            return Ok(response);
        }


        [HttpGet("fetch")]
        public async Task<ActionResult<ResponseModel>> FetchEmployeeData([FromQuery] int actionType, [FromQuery] string? param)
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
                    case 0: // Get Employee on Employee Code
                        if (string.IsNullOrEmpty(param))
                        {
                            response.IsSuccess = false;
                            response.Message = "Employee Code is required.";
                            return Ok(response);
                        }
                        response = await _employeeService.GetEmployeeByCode(param, clientId);
                        break;

                    case 1: // On Year All
                        response = await _employeeService.GetAllEmployeesByYear(param, clientId);
                        break;

                    case 2: // On SubDepartmentID, Year
                        response = await _employeeService.GetEmployeesBySubDept(param, clientId);
                        break;

                    case 3: // On Designation, Year
                        response = await _employeeService.GetEmployeesByDesignation(param, clientId);
                        break;

                    case 4: // On Status, Year
                        response = await _employeeService.GetEmployeesByStatus(param, clientId);
                        break;

                    case 5: // By Name list
                        response = await _employeeService.GetEmployeesByName(param, clientId);
                        break;

                    case 6: // By Table Fields
                        response = await _employeeService.GetEmployeesByField(param, clientId);
                        break;

                    case 7: // By Mobile Number
                        response = await _employeeService.GetEmployeesByMobile(param, clientId);
                        break;

                    case 8: // By Parentage
                        response = await _employeeService.GetEmployeesByParentage(param, clientId);
                        break;

                    case 9: // By Address
                        response = await _employeeService.GetEmployeesByAddress(param, clientId);
                        break;

                    case 10: // Employee Attendance Data
                        response = await _employeeService.GetEmployeesForAttendanceUpdate(param, clientId);
                        break;

                    case 11: // Get Employee Table Fields
                        response = await _employeeService.GetEmployeeTableFields(clientId);
                        break;
                    case 12: // Get Next Employee Code
                        response = await _employeeService.GetNextEmployeeCode(clientId);
                        break;

                    default:
                        response.IsSuccess = false;
                        response.Message = "Invalid actionType.";
                        break;
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error occurred while processing request.";
                response.Error = ex.Message;
                Repository.Error.ErrorBLL.CreateErrorLog("EmployeeController", "FetchEmployeeData", ex.ToString());
            }

            return Ok(response);
        }

    }
}
