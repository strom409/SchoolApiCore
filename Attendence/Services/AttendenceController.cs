using Attendence.Repository;
using Azure;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static System.Collections.Specialized.BitVector32;

namespace Attendence.Services
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttendenceController : ControllerBase
    {
        private readonly IAttendenceServices _attendanceService;

        public AttendenceController(IAttendenceServices attendanceService)
        {
            _attendanceService = attendanceService;
        }

        [HttpPost("add")]
        public async Task<ActionResult<ResponseModel>> AddAttendance([FromQuery] int actionType, [FromBody] object request)
        {
            var response = new ResponseModel { IsSuccess = true, Status = 0, Message = "Issue at Controller Level!" };

            try
            {
                var clientId = Request.Headers["X-Client-Id"].FirstOrDefault();
                if (string.IsNullOrEmpty(clientId))
                    return BadRequest("ClientId header missing");


                switch (actionType)
                {

                    case 0: // Add Single Attendance
                            // Deserialize request
                        var attendanceDto = JsonConvert.DeserializeObject<AttendanceDTO>(request.ToString());

                        response = await _attendanceService.AddAttendance(attendanceDto, clientId);
                        break;

                    case 1: // Add List of Attendance
                        {
                            var attendanceList = JsonConvert.DeserializeObject<List<AttendanceDTO>>(request.ToString());
                            response = await _attendanceService.AddAttendanceList(attendanceList, clientId);
                            break;
                        }

                    // You can extend this switch for future use cases like:
                    // case 1: // Add Bulk Attendance
                    // case 2: // Add Leave Request

                    default:
                        response.IsSuccess = false;
                        response.Message = "Invalid actionType for AddAttendance.";
                        break;
                }
            }
            catch (Exception ex)
            {
                Repository.Error.ErrorBLL.CreateErrorLog("AttendanceController", "AddAttendance", ex.ToString());
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "An unexpected error occurred.";
                response.Error = ex.Message;
            }

            return Ok(response);
        }

        [HttpPut("update")]
        public async Task<ActionResult<ResponseModel>> UpdateAttendance([FromQuery] int actionType, [FromBody] object request)
        {
            var response = new ResponseModel { IsSuccess = true, Status = 0, Message = "Issue at Controller Level!" };

            try
            {
                var clientId = Request.Headers["X-Client-Id"].FirstOrDefault();
                if (string.IsNullOrEmpty(clientId))
                    return BadRequest("ClientId header missing");

                // Deserialize the incoming object
                var attendanceDto = JsonConvert.DeserializeObject<AttendanceDTO>(request.ToString());

                switch (actionType)
                {
                    case 0: // Update today's attendance
                        response = await _attendanceService.UpdateTodaysAttendance(attendanceDto, clientId);
                        break;

                    // Future extensions can go here:
                    // case 1: // Update Bulk Attendance
                    // case 2: // Mark as Absent Reason

                    default:
                        response.IsSuccess = false;
                        response.Message = "Invalid actionType for UpdateAttendance.";
                        break;
                }
            }
            catch (Exception ex)
            {
                Repository.Error.ErrorBLL.CreateErrorLog("AttendanceController", "UpdateAttendance", ex.ToString());
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "An unexpected error occurred.";
                response.Error = ex.Message;
            }

            return Ok(response);
        }

        [HttpGet("attendance")]
        public async Task<ActionResult<ResponseModel>> FetchAttendance(int actionType, string? param)
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

            // Step 1: Handle null or bad param
            if (string.IsNullOrWhiteSpace(param))
                return BadRequest("param is required.");

            // Step 2: Split and trim the param string
            var paramList = param.Split(',').Select(p => p.Trim().Trim('"')).ToList();

            try
            {
                switch (actionType)
                {
                    case 0: // Get Today's Attendance
                        {
                            if (paramList.Count < 2)
                                return BadRequest("Expected: param = \"sessionId\", \"date\"");

                            var sessionId = paramList[0];
                            var date = paramList[1];

                            return Ok(await _attendanceService.GetTodaysAttendance(sessionId, date, clientId));
                        }

                    case 1: // Edit Attendance
                        {
                            if (paramList.Count < 2)
                                return BadRequest("Expected: param = \"sectionId\", \"date\"");

                            var sectionId = paramList[0];
                            var date = paramList[1];

                            return Ok(await _attendanceService.GetEditAttendance(sectionId, date, clientId));
                        }

                    case 2: // Absent List
                        {
                            if (paramList.Count < 2)
                                return BadRequest("Expected: param = \"sectionId\", \"date\"");

                            var sectionId = paramList[0];
                            var date = paramList[1];

                            return Ok(await _attendanceService.GetAbsentList(sectionId, date, clientId));
                        }

                    case 3: // Attendance Between Dates
                        {
                            if (paramList.Count < 3)
                                return BadRequest("Expected: param = \"dateFrom\", \"dateTo\", \"session\"");

                            var dateFrom = paramList[0];
                            var dateTo = paramList[1];
                            var session = paramList[2];

                            return Ok(await _attendanceService.getAttendanceListOnDates(dateFrom, dateTo, session, clientId));
                        }

                    case 4: // Check Attendance Added
                        {
                            if (paramList.Count < 3)
                                return BadRequest("Expected: param = \"classId,sectionId,date\"");

                            var classId = paramList[0];
                            var sectionId = paramList[1];
                            var date = paramList[2];

                            return Ok(await _attendanceService.CheckAttendanceAddedorNot(classId, sectionId, date, clientId));
                        }

                    case 5: // Monthly Attendance
                        {
                            if (paramList.Count < 5)
                                return BadRequest("Expected: param = \"session,dateFrom,dateTo,className,sectionName\"");

                            var session = paramList[0];
                            var dateFrom = paramList[1];
                            var dateTo = paramList[2];
                            var className = paramList[3];
                            var sectionName = paramList[4];

                            return Ok(await _attendanceService.GetMonthlyAttendance(session, dateFrom, dateTo, className, sectionName, clientId));
                        }


                    case 6: // Attendance With ClassId
                        {
                            if (paramList.Count < 3)
                                return BadRequest("Missing or invalid parameters. Expected: \"dateFrom\", \"dateTo\", \"classId\", \"sectionId\".");


                            var dateFrom = paramList[0];
                            var dateTo = paramList[1];
                            var classId = paramList[2];
                            var sectionId = paramList[3];

                            return Ok(await _attendanceService.getAttendanceListOnDateswithclassid(dateFrom, dateTo, classId, sectionId, clientId));
                        }

                    case 7: // Pending Attendance Students
                        {
                            if (paramList.Count < 4)
                                return BadRequest("Expected: param = \"classId\", \"sectionId\", \"session\", \"date\"");

                            var classId = paramList[0];
                            var sectionId = paramList[1];
                            var session = paramList[2];
                            var date = paramList[3];

                            return Ok(await _attendanceService.GetPendingAttendanceStudents(classId, sectionId, session, date, clientId));
                        }
                    case 8: // Attendance Marked Or Not As Per Date
                        {
                            if (string.IsNullOrWhiteSpace(param))
                                return BadRequest("Expected: param = \"session,fromDate,toDate\"");

                            return Ok(await _attendanceService.GetAttendanceMarkedOrNotAsPerDate(param, clientId));
                        }

                    case 9: // Attendance Report (Pivot) using SP
                        {
                            if (paramList.Count < 4)
                                return BadRequest("Expected: param = \"classId, sectionId, dateFrom, dateTo\"");

                            var classId = paramList[0];
                            var sectionId = paramList[1];
                            var dateFrom = paramList[2];
                            var dateTo = paramList[3];

                            return Ok(await _attendanceService.GetAttendanceReport(classId, sectionId, dateFrom, dateTo, clientId));
                        }

                    default:
                        return BadRequest("Invalid actionType.");
                }
            }
            catch (Exception ex)
            {
                Repository.Error.ErrorBLL.CreateErrorLog("AttendanceController", "FetchAttendance", ex.ToString());
                response.IsSuccess = false;
                response.Status = -1;
                response.Error = ex.Message;
                return StatusCode(500, response);
            }
        }


    }
}
