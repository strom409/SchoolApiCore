using HR.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HR.Services.Deparments
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentsController : ControllerBase
    {

        private readonly IDepartmentsService _departmentService;

        public DepartmentsController(IDepartmentsService departmentService)
        {
            _departmentService =departmentService;  
        }

        [HttpPost("department-add")]
        public async Task<ActionResult<ResponseModel>> DepartmentAdd([FromQuery] int actionType, [FromBody] SubDepartment value)
        {
            #region Get ClientId
            var clientId = Request.Headers["X-Client-Id"].FirstOrDefault() ?? "client1";
            if (string.IsNullOrEmpty(clientId))
                return BadRequest("ClientId header missing");
            #endregion

            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "Invalid action type",
                ResponseData = null
            };

            try
            {
                switch (actionType)
                {
                    case 0: // Add Department
                        response = await _departmentService.AddDepartment(value, clientId);
                        break;

                    default:
                        response.IsSuccess = false;
                        response.Message = "Invalid action type for Department.";
                        break;
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error: " + ex.Message;
                Repository.Error.ErrorBLL.CreateErrorLog("DepartmentController", "DepartmentAction", ex.ToString());
            }

            return Ok(response);
        }
        [HttpPut("update-department")]
        public async Task<ActionResult<ResponseModel>> UpdateDepartment([FromQuery] int actionType,
    [FromBody] SubDepartment value)
        {
            #region Get ClientId
            var clientId = Request.Headers["X-Client-Id"].FirstOrDefault() ?? "client1";
            if (string.IsNullOrEmpty(clientId))
                return BadRequest("ClientId header missing");
            #endregion

            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "Invalid action type",
                ResponseData = null
            };

            try
            {
                switch (actionType)
                {
                    case 0: // Update Department
                        response = await _departmentService.UpdateDepartment(value, clientId);
                        break;

                    default:
                        response.IsSuccess = false;
                        response.Message = "Invalid action type for update operation.";
                        break;
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error: " + ex.Message;
                Repository.Error.ErrorBLL.CreateErrorLog("DepartmentController", "UpdateDepartment", ex.ToString());
            }

            return Ok(response);
        }

        [HttpGet("get-department-data")]
        public async Task<ActionResult<ResponseModel>> GetDepartmentData([FromQuery] int actionType,
    [FromQuery] long id)
        {
            #region Get ClientId
            var clientId = Request.Headers["X-Client-Id"].FirstOrDefault() ?? "client1";
            if (string.IsNullOrEmpty(clientId))
                return BadRequest("ClientId header missing");
            #endregion

            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "Invalid action type!",
                ResponseData = null
            };

            try
            {
                switch (actionType)
                {

                    case 0:
                        // Get Department by ID
                        response = await _departmentService.getDepartments(clientId);
                        break;
                    case 1:
                        // Get Department by ID
                        response = await _departmentService.GetDepartmentById(id, clientId);
                        break;

                    // Future action types can be handled here
                    default:
                        response.IsSuccess = false;
                        response.Message = "Unsupported action type!";
                        break;
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error: " + ex.Message;
                Repository.Error.ErrorBLL.CreateErrorLog("DepartmentController", "GetDepartmentData", ex.ToString());
            }

            return Ok(response);
        }

        [HttpDelete("delete-department")]
        public async Task<ActionResult<ResponseModel>> DeleteDepartment([FromQuery] int actionType,
    [FromQuery] long id)
        {
            #region Get ClientId
            var clientId = Request.Headers["X-Client-Id"].FirstOrDefault() ?? "client1";
            if (string.IsNullOrEmpty(clientId))
                return BadRequest("ClientId header missing");
            #endregion

            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "Invalid action type",
                ResponseData = null
            };

            try
            {
                switch (actionType)
                {
                    case 0: // Delete by ID
                        response = await _departmentService.DeleteDepartment(id, clientId);
                        break;

                    default:
                        response.IsSuccess = false;
                        response.Message = "Invalid action type for delete operation.";
                        break;
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error: " + ex.Message;
                Repository.Error.ErrorBLL.CreateErrorLog("DepartmentController", "DeleteDepartment", ex.ToString());
            }

            return Ok(response);
        }

    }
}
