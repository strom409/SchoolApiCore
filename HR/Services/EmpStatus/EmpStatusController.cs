using HR.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HR.Services.EmpStatus
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmpStatusController : ControllerBase
    {
       private readonly IEmpStatusService _empStatusservice;
        public EmpStatusController(IEmpStatusService empStatusservice)
        {
            _empStatusservice=empStatusservice;
        }
        [HttpGet("get-employee-status")]
        public async Task<ActionResult<ResponseModel>> GetEmployeeStatus([FromQuery] int actionType)
        {
            var clientId = Request.Headers["X-Client-Id"].FirstOrDefault() ?? "client1";
            var response = new ResponseModel { IsSuccess = true, Status = 0, Message = "Issue at Controller Level !" };

            try
            {
                switch (actionType)
                {
                    case 0:
                        response = await _empStatusservice.GetEmployeeStatus(clientId);
                        break;

                    default:
                        response.IsSuccess = false;
                        response.Message = "Invalid actionType";
                        break;
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error: " + ex.Message;
                Repository.Error.ErrorBLL.CreateErrorLog("EmployeeStatusController", "GetEmployeeStatus", ex.ToString());
            }

            return Ok(response);
        }
    }
}
