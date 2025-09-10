using HR.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HR.Services.Subjects
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeSubjectsController : ControllerBase
    {
        private readonly IEmployeeSubjectsService _employeeSubjectsService;
        public EmployeeSubjectsController(IEmployeeSubjectsService employeeSubjectsService)
        {
            _employeeSubjectsService = employeeSubjectsService;
        }

        [HttpPost("add")]
        public async Task<IActionResult> Add([FromQuery] int actionType, [FromBody] EmployeeSubjects request)
        {
            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "Issue at Controller Level !"
            };
            #region Get ClientId from Header
            var clientId = Request.Headers["X-Client-Id"].FirstOrDefault();
            if (string.IsNullOrEmpty(clientId))
                return BadRequest("ClientId header missing");
            #endregion

            try
            {
                #region Switch ActionType
                switch (actionType)
                {
                    case 1:
                        response = await _employeeSubjectsService.AddEmployeeSubject(request, clientId);
                        break;

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
                Repository.Error.ErrorBLL.CreateErrorLog("EmployeeSubjectsController", "Add", ex.ToString());
                #endregion
            }

            #region Return Response
            return Ok(response);
            #endregion
        }



        [HttpPost("update")]
        public async Task<IActionResult> Update([FromQuery] int actionType, [FromBody] EmployeeSubjects request)
        {
            #region Initialize Response
            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "Issue at Controller Level !"
            };
            #endregion

            #region Get ClientId from Header
            var clientId = Request.Headers["X-Client-Id"].FirstOrDefault();
            if (string.IsNullOrEmpty(clientId))
                return BadRequest("ClientId header missing");
            #endregion

            try
            {
                #region Switch ActionType
                switch (actionType)
                {
                    case 2:
                        response = await _employeeSubjectsService.UpdateEmployeeSubject(request, clientId);
                        break;

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
                Repository.Error.ErrorBLL.CreateErrorLog("EmployeeSubjectsController", "Update", ex.ToString());
                #endregion
            }

            #region Return Response
            return Ok(response);
            #endregion
        }



        [HttpPost("delete")]
        public async Task<IActionResult> Delete([FromQuery] int actionType, [FromBody] EmployeeSubjects request)
        {
            #region Initialize Response
            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "Issue at Controller Level !"
            };
            #endregion

            #region Get ClientId from Header
            var clientId = Request.Headers["X-Client-Id"].FirstOrDefault();
            if (string.IsNullOrEmpty(clientId))
                return BadRequest("ClientId header missing");
            #endregion

            try
            {
                #region Switch ActionType
                switch (actionType)
                {
                    case 3:
                        response = await _employeeSubjectsService.DeleteEmployeeSubject(request.ESID.ToString(), clientId);
                        break;

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
                Repository.Error.ErrorBLL.CreateErrorLog("EmployeeSubjectsController", "Delete", ex.ToString());
                #endregion
            }

            #region Return Response
            return Ok(response);
            #endregion
        }


        [HttpGet("get")]
        public async Task<IActionResult> Get(int actionType, [FromQuery] string? param)
        {
            #region Initialize Response
            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "Issue at Controller Level !"
            };
            #endregion

            #region Get ClientId from Header
            var clientId = Request.Headers["X-Client-Id"].FirstOrDefault();
            if (string.IsNullOrEmpty(clientId))
                return BadRequest("ClientId header missing");
            #endregion

            try
            {
                #region Switch ActionType
                switch (actionType)
                {
                    case 0: // Get by ID
                        response = await _employeeSubjectsService.GetEmployeeSubjectById(param, clientId);
                        break;

                    case 1: // Get all
                        response = await _employeeSubjectsService.GetEmployeeSubjects(clientId);
                        break;

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
                Repository.Error.ErrorBLL.CreateErrorLog("EmployeeSubjectsController", "Get", ex.ToString());
                #endregion
            }

            #region Return Response
            return Ok(response);
            #endregion
        }

    }
}

