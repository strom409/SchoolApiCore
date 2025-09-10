
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TeacherLog.Repository;

namespace TeacherLog.Services
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeacherLogController : ControllerBase
    {
        private readonly ITeacherLogServices _teacherLogServices;
        public TeacherLogController(ITeacherLogServices teacherLogServices)
        {
            _teacherLogServices = teacherLogServices;
        }


        [HttpPost("post")]
        public async Task<ActionResult<ResponseModel>> Post([FromQuery] int actionType, [FromBody] object request)
        {
            var response = new ResponseModel { IsSuccess = true, Status = 0, Message = "Issue at Controller Level!" };

            try
            {
                var clientId = Request.Headers["X-Client-Id"].FirstOrDefault() ?? "client1";
                if (string.IsNullOrEmpty(clientId))
                    return BadRequest("ClientId header missing.");

                switch (actionType)
                {
                    case 1: // Add New Teacher Log
                        var log1 = JsonConvert.DeserializeObject<TeacherLogData>(request.ToString());
                        response = await _teacherLogServices.AddTeacherLogDataOnSectionIDandDate(log1, clientId);
                        break;

                    case 2: // Add Teacher LOG for New TINY School
                        var log2 = JsonConvert.DeserializeObject<TeacherLogData>(request.ToString());
                        response = await _teacherLogServices.AddTeacherLogForNewTiny(log2, clientId);
                        break;

                    case 3: // Add Teacher Performance
                        var log3 = JsonConvert.DeserializeObject<TeacherLogData>(request.ToString());
                        response = await _teacherLogServices.AddTeacherPerformance(log3, clientId);
                        break;

                    default:
                        response.IsSuccess = false;
                        response.Message = "Invalid actionType provided.";
                        break;
                }
            }
            catch (Exception ex)
            {
                Repository.Error.ErrorBLL.CreateErrorLog("TeacherLogController", "Post", ex.ToString());
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "An unexpected error occurred.";
                response.Error = ex.Message;
            }

            return Ok(response);
        }


        [HttpPut("update")]
        public async Task<ActionResult<ResponseModel>> UpdateTeacherLog([FromQuery] int actionType, [FromBody] object request)
        {
            var response = new ResponseModel { IsSuccess = true, Status = 0, Message = "Issue at Controller Level!" };

            try
            {
                // Get client ID from header
                var clientId = Request.Headers["X-Client-Id"].FirstOrDefault();
                if (string.IsNullOrEmpty(clientId))
                    return BadRequest("ClientId header missing.");

                switch (actionType)
                {

                    case 0: // Update Teacher Log for New Tiny School
                        var updateLog = JsonConvert.DeserializeObject<TeacherLogData>(request.ToString());
                        response = await _teacherLogServices.UpdateTeacherLog(updateLog, clientId);
                        break;

                    default:
                        response.IsSuccess = false;
                        response.Message = "Invalid actionType provided.";
                        break;
                }
            }
            catch (Exception ex)
            {
                Repository.Error.ErrorBLL.CreateErrorLog("TeacherLogController", "UpdateTeacherLog", ex.ToString());
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "An unexpected error occurred.";
                response.Error = ex.Message;
            }

            return Ok(response);
        }

        [HttpGet("fetch")]
        public async Task<ActionResult<ResponseModel>> GetTeacherLogData([FromQuery] int actionType, [FromQuery] string? param)
        {
            var response = new ResponseModel { IsSuccess = true, Status = 0, Message = "Issue at Controller Level!" };

            try
            {
                var clientId = Request.Headers["X-Client-Id"].FirstOrDefault();
                if (string.IsNullOrEmpty(clientId))
                    return BadRequest("ClientId header missing.");

                switch (actionType)
                {
                    case 0:
                        response = await _teacherLogServices.GetTeacherLogDataOnSectionIDandDate(param, clientId);
                        break;
                    case 1:
                        response = await _teacherLogServices.GetTeacherLogDataOnECodeandDate(param, clientId);
                        break;
                    case 2:
                        //not in use
                        response = await _teacherLogServices.GetSubjectList(param, clientId);
                        break;
                    case 3:
                        response = await _teacherLogServices.GetSubSubjectList(param, clientId);
                        break;
                    case 4:
                        response = await _teacherLogServices.GetOptSubjectList(param, clientId);
                        break;
                    case 5:
                        response = await _teacherLogServices.GetTeacherLogOnDateList(param, clientId);
                        break;
                    case 6:
                        response = await _teacherLogServices.GetTeacherLogOnDateAndCodeList(param, clientId);
                        break;
                    case 7:
                        response = await _teacherLogServices.GetTeachersLog(param, clientId);
                        break;
                    case 8:
                        response = await _teacherLogServices.GetTeachersLogfromTT(param, clientId);
                        break;
                    case 9:
                        response = await _teacherLogServices.GetTeachersLogFromTTEmpty(param, clientId);
                        break;
                    case 10:
                        response = await _teacherLogServices.GetTeacherPerformance(clientId);
                        break;
                    case 11:                                 
                        response = await _teacherLogServices.GetTeachersLogRangeWise(param, clientId);
                        break;
                    default:
                        response.IsSuccess = false;
                        response.Message = "Invalid actionType provided.";
                        break;
                }
            }
            catch (Exception ex)
            {
                Repository.Error.ErrorBLL.CreateErrorLog("TeacherLogController", "GetTeacherLogData", ex.ToString());
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "An unexpected error occurred.";
                response.Error = ex.Message;
            }

            return Ok(response);
        }

        [HttpDelete("delete")]
        public async Task<ActionResult<ResponseModel>> DeleteTeacherLog([FromQuery] int actionType, [FromBody] object request)
        {
            var response = new ResponseModel { IsSuccess = true, Status = 0, Message = "Issue at Controller Level!" };

            try
            {
                // Get client ID from header or fallback default
                var clientId = Request.Headers["X-Client-Id"].FirstOrDefault();
                if (string.IsNullOrEmpty(clientId))
                    return BadRequest("ClientId header missing.");

                switch (actionType)
                {
                    case 0: 
                        var deleteLog = JsonConvert.DeserializeObject<TeacherLogData>(request.ToString());
                        response = await _teacherLogServices.DeleteTeacherLogOnLogID(deleteLog, clientId);
                        break;

                    default:
                        response.IsSuccess = false;
                        response.Message = "Invalid actionType provided.";
                        break;
                }
            }
            catch (Exception ex)
            {
                Repository.Error.ErrorBLL.CreateErrorLog("TeacherLogDeleteController", "DeleteTeacherLog", ex.ToString());
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "An unexpected error occurred.";
                response.Error = ex.Message;
            }

            return Ok(response);
        }
    }
}

