using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Student.Repository;
using Student.Services.Students;
using System.Data;
using System.Text.Json;
using static System.Collections.Specialized.BitVector32;

namespace SchoolApiCore.Controllers
{

    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly IStudentService _studentService;
     

        public StudentController(IStudentService studentService)
        {
            _studentService = studentService;
           
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="actionType"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("AddStudent")]
        public async Task<ActionResult<ResponseModel>> AddStudent([FromQuery] int actionType, [FromForm] AddStudentRequestDTO request)
        {
            var response = new ResponseModel { IsSuccess = true, Status = 0, Message = "Issue at Controller Level !" };

            try
            {
                 var clientId = Request.Headers["X-Client-Id"].FirstOrDefault();


                if (string.IsNullOrEmpty(clientId))
                    return BadRequest("ClientId header missing");

                switch (actionType)
                {
                    case 0:
                        response = await _studentService.AddStudent(request, clientId);
                        break;
                    case 1:
                        response = await _studentService.AddNewStudentWithRegNo(request, clientId);
                        break;
                    case 2:
                        response = await _studentService.AddNewGPS(request, clientId);
                        break;

                    default:
                        response.Message = "Invalid actionType.";
                        return Ok(response);
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error: " + ex.ToString();
                Student.Repository.Error.ErrorBLL.CreateErrorLog("StudentController", "AddStudentByActionType", ex.ToString());
                return Ok(response);
            }

            return Ok(response);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="actionType"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpGet("student")]
        public async Task<ActionResult<ResponseModel>> FetchStudent(int actionType, string? param)
        {
            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "Issue at Controller Level !",

            };
             var clientId = Request.Headers["X-Client-Id"].FirstOrDefault();

            //  var clientId = Request.Headers["X-Client-Id"].FirstOrDefault();
           // var clientId = "client1";

            if (string.IsNullOrEmpty(clientId))
                return BadRequest("ClientId header missing");
            try
            {
                switch (actionType)
                {
                    case 0:
                        if (!long.TryParse(param, out long classId))
                            return BadRequest(new ResponseModel { IsSuccess = true, Status = 0 });

                        return Ok(await _studentService.GetStudentsByClass(classId, clientId));

                    case 1:
                        if (string.IsNullOrEmpty(param))
                            return BadRequest(new ResponseModel { IsSuccess = true, Status = 0 });

                        return Ok(await _studentService.GetStudentByAdmissionNo(param, clientId));

                    case 2:
                        if (string.IsNullOrEmpty(param))
                            return BadRequest(new ResponseModel { IsSuccess = true, Status = 0 });

                        return Ok(await _studentService.GetStudentsByName(param, clientId));

                    case 3:
                        if (!long.TryParse(param, out long studentInfoId))
                            return BadRequest(new ResponseModel { IsSuccess = true, Status = 0 });

                        return Ok(await _studentService.GetStudentByStudentInfoId(studentInfoId, clientId));

                    case 4:
                        if (string.IsNullOrEmpty(param))
                            return BadRequest(new ResponseModel { IsSuccess = true, Status = 0 });

                        return Ok(await _studentService.GetStudentByPhone(param, clientId));

                    case 5:
                        if (string.IsNullOrEmpty(param))
                            return BadRequest(new ResponseModel { IsSuccess = true, Status = 0 });

                        return Ok(await _studentService.GetStudentsByCurrentSession(param, clientId));

                    case 6:
                        return Ok(await _studentService.GetNextAdmissionNoAsync(clientId));


                    case 7:
                        return Ok(await _studentService.GetAllStudentsOnSectionID(param, clientId));

                    case 8:
                        if (!long.TryParse(param, out long activeClassId))
                            return BadRequest(new ResponseModel { IsSuccess = true, Status = 0 });
                        // Get Only Active Students on Class ID in Current Session
                        return Ok(await _studentService.GetOnlyActiveStudentsOnClassID(activeClassId, clientId));
                    case 9:
                        if (!long.TryParse(param, out long activeSectionId))
                            return BadRequest(new ResponseModel { IsSuccess = true, Status = 0 });
                        // Get Only Active Students on Section ID in Current Session
                        return Ok(await _studentService.GetOnlyActiveStudentsOnSectionID(activeSectionId, clientId));
                    case 10: // 
                        return Ok(await _studentService.GetMaxRollno(param, clientId));

                    case 12: // Get All List of Discharged Students on SectionID for General Schools
                        return Ok(await _studentService.GetAllDischargedStudentsOnSectionID(param, clientId));
                    case 13: // Get Total Student Roll in Current Session for dash Board
                        return Ok(await _studentService.TotalStudentsRollForDashBoard(param, clientId));
                    case 14: // Get Class Wise Student Roll in Current Session for dash Board
                        return Ok(await _studentService.ClassWisStudentsRollForDashBoard(param, clientId));
                    case 15: // Get Total Student Roll in Current Session for dash Board
                        return Ok(await _studentService.TotalStudentsRollForDashBoardOnDate(param, clientId));
                    case 16: // Get Section Wise Student Roll With Attendance  on ClassID for dash Board
                        return Ok(await _studentService.SectionWisStudentsRollWithAttendanceForDashBoard(param, clientId));

                    case 18: // Get Student List in with Board Numbers on SectionID
                        return Ok(await _studentService.GetBoardNoWithDate(param, clientId));
                    case 19: // Get GetAllSessions 
                        return Ok(await _studentService.GetAllSessions(clientId));
                    case 20: // 🔹 New case: Get StudentID from StudentInfoID
                        if (string.IsNullOrEmpty(param))
                            return BadRequest(new ResponseModel { IsSuccess = true, Status = 0 });
                        return Ok(await _studentService.GetStudentIdAsync(param, clientId));

                    default:
                        return BadRequest(new ResponseModel { IsSuccess = true, Status = 0 });
                }
            }
            catch (Exception ex)
            {
                Student.Repository.Error.ErrorBLL.CreateErrorLog("StudentController", "FetchStudentInfo", ex.ToString());
                response.Status = -1;
                response.Error = ex.Message;
                return StatusCode(500, response);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="actionType"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut("update-student")]
        public async Task<IActionResult> UpdateStudent(int actionType, [FromForm] UpdateStudentRequestDTO request)
        {
            try
            {
                
                var response = new ResponseModel { IsSuccess = true, Status = 0, Message = "Issue at Controller Level !" };
              
                
                
                var clientId = Request.Headers["X-Client-Id"].FirstOrDefault();

                if (string.IsNullOrEmpty(clientId))
                    return Unauthorized("ClientId  missing");
                
                switch (actionType)
                {
                    case 0:
                        response = await _studentService.UpdateStudentAsync(request, clientId);
                        break;

                    case 1:
                        response = await _studentService.UpdateParentDetail(request, clientId);
                        break;

                    case 2:
                        response = await _studentService.UpdateAddressDetail(request, clientId);
                        break;

                    case 3:
                        response = await _studentService.UpdatePersonalDetail(request, clientId);
                        break;

                    case 4:
                        response = await _studentService.UpdateStudentRollNo(request, clientId);
                        break;


                    case 5:
                        response = await _studentService.UpdateBoardNo(request, clientId);
                        break;
                    case 6:
                        response = await _studentService.UpdateDOB(request, clientId);
                        break;

                    case 7:
                        response = await _studentService.UpdateSection(request, clientId);
                        break;

                    case 8:
                        response = await _studentService.UpdateClass(request, clientId);
                        break;

                    case 9: // Discharge Student AND SET IsDischarged='True' of StudentInfo Table as itz Bit
                        response = await _studentService.DischargeStudent(request, clientId);
                        break;
                    case 10: // Discharge Student AND SET IsDischarged=1 of StudentInfo Table as itz Int

                        response = await _studentService.DischargeStudentForIntValue(request, clientId);
                        break;

                    case 11: // Discharge Student AND SET IsDischarged=1 of StudentInfo Table as itz Int

                        response = await _studentService.RejoinStudent(request, clientId);
                        break;

                    case 12: // Discharge Student AND SET IsDischarged=1 of StudentInfo Table as itz Int

                        response = await _studentService.RejoinStudentForIntValue(request, clientId);
                        break;

                    case 13: // Discharge Student AND SET IsDischarged=1 of StudentInfo Table as itz Int

                        response = await _studentService.UpdateStudentEducationAdmissionPrePrimaryEtc(request, clientId);
                        break;

                    case 14: // Discharge Student AND SET IsDischarged=1 of StudentInfo Table as itz Int

                        response = await _studentService.UpdateStudentHeightWeightAdharNamePENEtcUDISE(request, clientId);
                        break;
             
                    //case 16:
                    //    try
                    //    {
                    //        // Initialize empty updates list
                    //        var updates = new List<StudentRollNoUpdate>();

                    //        // Handle bulk updates
                    //        if (request.BulkUpdates != null)
                    //        {
                    //            updates = request.BulkUpdates
                    //                .Where(u => u != null && u.StudentInfoID > 0 && !string.IsNullOrWhiteSpace(u.RollNo))
                    //                .ToList();
                    //        }
                    //        // Handle single update
                    //        else if (!string.IsNullOrWhiteSpace(request.StudentInfoID) &&
                    //                int.TryParse(request.StudentInfoID, out var studentId) &&
                    //                studentId > 0 &&
                    //                !string.IsNullOrWhiteSpace(request.RollNo))
                    //        {
                    //            updates.Add(new StudentRollNoUpdate
                    //            {
                    //                StudentInfoID = studentId,
                    //                RollNo = request.RollNo,
                    //                UpdatedBy = request.UpdatedBy ?? "System"
                    //            });
                    //        }

                    //        if (updates.Count == 0)
                    //        {
                    //            response = new ResponseModel
                    //            {
                    //                IsSuccess = false,
                    //                Message = "No valid roll number updates provided"
                    //            };
                    //            break;
                    //        }

                    //        response = await _studentService.UpdateClassStudentRollNumbers(updates, clientId);
                    //    }
                    //    catch (Exception ex)
                    //    {
                    //        response = new ResponseModel
                    //        {
                    //            IsSuccess = false,
                    //            Message = $"Roll number update failed: {ex.Message}"
                    //        };
                    //        // Log error if needed
                    //      Student.Repository.Error.ErrorBLL.CreateErrorLog("StudentController", "UpdateRollNumbers", ex.ToString());
                    //    }
                    //    break;

                    default:
                        response = new ResponseModel
                        {
                            IsSuccess = false,
                            Status = 0,
                            Message = "Invalid action type."
                        };
                        break;
                }

                if (response.IsSuccess)
                    return Ok(response);
                else
                    return BadRequest(response);
            }
            catch (Exception ex)
            {
                Student.Repository.Error.ErrorBLL.CreateErrorLog("StudentController", "UpdateStudentByActionType", ex.ToString());

                return StatusCode(500, new ResponseModel
                {
                    IsSuccess = false,
                    Status = -1,
                    Error = ex.Message
                });
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="actionType"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut("bulk-update-students")]
        public async Task<IActionResult> BulkUpdateStudents([FromQuery] int actionType, [FromBody] object request)
        {
            var response = new ResponseModel { IsSuccess = true, Status = 0, Message = "Issue at Controller Level !" };

            try
            {
                // Get ClientId from Headers 
                var clientId = Request.Headers["X-Client-Id"].FirstOrDefault();
                if (string.IsNullOrEmpty(clientId))
                    return Unauthorized("ClientId missing");

                switch (actionType)
                {
                    case 0: // Bulk RollNo Update
                        {
                            var rollUpdateRequest = JsonConvert.DeserializeObject<BulkRollNoUpdateRequest>(
                                 ((JsonElement)request).GetRawText()
                            );

                            if (rollUpdateRequest?.BulkUpdates == null || !rollUpdateRequest.BulkUpdates.Any())
                            {
                                return BadRequest(new ResponseModel
                                {
                                    IsSuccess = false,
                                    Message = "No roll number updates provided"
                                });
                            }

                            var updates = rollUpdateRequest.BulkUpdates
                                .Where(u => u.StudentInfoID.HasValue &&
                                            u.StudentInfoID.Value > 0 &&
                                            !string.IsNullOrWhiteSpace(u.RollNo))
                                .ToList();

                            if (!updates.Any())
                            {
                                return BadRequest(new ResponseModel
                                {
                                    IsSuccess = false,
                                    Message = "No valid roll number updates found"
                                });
                            }

                            response = await _studentService.UpdateClassStudentRollNumbers(updates, clientId);
                            break;
                        }

                    case 15: // Bulk Session Update
                        {
                            string rawJson;

                            // Handle both possible types
                            if (request is JsonElement element)
                            {
                                rawJson = element.GetRawText();
                            }
                            else if (request is JObject jObject)
                            {
                                rawJson = jObject.ToString();
                            }
                            else
                            {
                                rawJson = request.ToString();
                            }

                            var sessionRequest = JsonConvert.DeserializeObject<StudentSessionUpdateRequest>(rawJson);

                            if (sessionRequest?.Students == null || !sessionRequest.Students.Any())
                            {
                                return BadRequest(new ResponseModel
                                {
                                    IsSuccess = false,
                                    Message = "No session update data provided"
                                });
                            }

                            response = await _studentService.UpdateStudentSessionAsync(sessionRequest, clientId);
                            break;
                        }


                    default:
                        response = new ResponseModel
                        {
                            IsSuccess = false,
                            Message = "Invalid ActionType"
                        };
                        break;
                }

                return response.IsSuccess ? Ok(response) : BadRequest(response);
            }
            catch (Exception ex)
            {
                Student.Repository.Error.ErrorBLL.CreateErrorLog(
                    "StudentRollNoController", "BulkUpdateStudents", ex.ToString());

                return StatusCode(500, new ResponseModel
                {
                    IsSuccess = false,
                    Status = -1,
                    Error = ex.Message
                });
            }
        }


    }
}







