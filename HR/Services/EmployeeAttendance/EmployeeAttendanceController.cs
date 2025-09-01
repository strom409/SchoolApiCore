using HR.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HR.Services.EmployeeAttendance
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeAttendanceController : ControllerBase
    {
        private readonly IEmployeeAttendanceService _employeeAttendanceService;
        public EmployeeAttendanceController(IEmployeeAttendanceService employeeAttendanceService)
        {
            _employeeAttendanceService= employeeAttendanceService;
        }

        [HttpPost]
        public async Task<ActionResult<ResponseModel>> Post([FromQuery] int actionType, [FromBody] EmpAttendance value)
        {
            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "Issue at Controller Level !"
            };

            var clientId = Request.Headers["X-Client-Id"].FirstOrDefault() ?? "client1";
            if (string.IsNullOrEmpty(clientId))
                return BadRequest("ClientId header missing");
            try
            {
                switch (actionType)
                {
                    case 0:
                        response = await _employeeAttendanceService.AddEmployeeAttendance(value, clientId);
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
                Repository.Error.ErrorBLL.CreateErrorLog("EmployeeAttendanceController", "Post", ex.ToString());
            }

            return Ok(response);
        }

        [HttpPut]
        public async Task<ActionResult<ResponseModel>> Put([FromQuery] int actionType, [FromBody] EmpAttendance value)
        {
            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "Issue at Controller Level !"
            };

            var clientId = Request.Headers["X-Client-Id"].FirstOrDefault() ?? "client1";
            if (string.IsNullOrEmpty(clientId))
                return BadRequest("ClientId header missing");

            try
            {
                switch (actionType)
                {
                    case 0: // Update Employee Attendance
                        response = await _employeeAttendanceService.UpdateEmployeeAttendance(value, clientId);
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
                Repository.Error.ErrorBLL.CreateErrorLog("EmployeeAttendanceController", "Put", ex.ToString());
            }

            return Ok(response);
        }

        [HttpGet]
        public async Task<ActionResult<ResponseModel>> Get([FromQuery] int actionType,[FromQuery] string param)
        {
            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "Issue at Controller Level !"
            };

            var clientId = Request.Headers["X-Client-Id"].FirstOrDefault() ?? "client1";
            if (string.IsNullOrEmpty(clientId))
                return BadRequest("ClientId header missing");


            try
            {
                if (string.IsNullOrEmpty(param))
                {
                    response.Message = "Parameter 'param' is required";
                    return BadRequest(response);
                }

                switch (actionType)
                {
                    case 0:
                        // Expected format: "empCode|fromDate-toDate"
                        response = await _employeeAttendanceService.GetEmpAttendanceByCodeOnDateRange(param, clientId);
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
                Repository.Error.ErrorBLL.CreateErrorLog("EmployeeAttendanceController", "Get", ex.ToString());
            }

            return Ok(response);
        }

        [HttpDelete]
        public async Task<ActionResult<ResponseModel>> Delete([FromQuery] int actionType, [FromBody] EmpAttendance value)
        {
            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "Issue at Controller Level !"
            };

            var clientId = Request.Headers["X-Client-Id"].FirstOrDefault() ?? "client1";
            if (string.IsNullOrEmpty(clientId))
                return BadRequest("ClientId header missing");

            try
            {
                switch (actionType)
                {
                    case 0: // Delete Employee Attendance
                        response = await _employeeAttendanceService.DeleteEmployeeAttendance(value, clientId);
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
                Repository.Error.ErrorBLL.CreateErrorLog("EmployeeAttendanceController", "Delete", ex.ToString());
            }

            return Ok(response);
        }


    }
}
