using ClassTest.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ClassTest.Services.Subject
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubjectController : ControllerBase
    {
        private readonly ISubjectService _subjectService;

        public SubjectController(ISubjectService subjectService)
        {
            _subjectService = subjectService;
        }


        [HttpPost("subject-info")]
        public async Task<ActionResult<ResponseModel>> AddSubject([FromQuery] int actionType, [FromBody] SubjectDTO value)
        {
            var response = new ResponseModel { IsSuccess = true, Status = 0 };
            var clientId = Request.Headers["X-Client-Id"].FirstOrDefault();

            if (string.IsNullOrEmpty(clientId))
                return BadRequest("ClientId header missing");

            try
            {
                switch (actionType)
                {
                    case 0: // Add Subject
                        return Ok(await _subjectService.InsertNewSubject(value, clientId));

                    case 1: // Add Optional Subject
                        return Ok(await _subjectService.InsertNewOptionalSubject(value, clientId));

                    case 2: // Add SubSubject
                        return Ok(await _subjectService.InsertNewSubSubject(value, clientId));
                    case 3: // Add SubSubject
                        return Ok(await _subjectService.UpgradeSubjectAsync(value, clientId));
                        

                    default:
                        response.Message = "Invalid actionType";
                        return BadRequest(response);
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Status = -1;
                response.Error = ex.Message;
                Repository.Error.ErrorBLL.CreateErrorLog("SubjectController", "AddOrUpdateSubject", ex.ToString());
                return StatusCode(500, response);
            }
        }

        [HttpPut("subject-info")]
        public async Task<ActionResult<ResponseModel>> UpdateSubject([FromQuery] int actionType, [FromBody] SubjectDTO value)
        {
            var response = new ResponseModel { IsSuccess = true, Status = 0 };
            var clientId = Request.Headers["X-Client-Id"].FirstOrDefault();

            if (string.IsNullOrEmpty(clientId))
                return BadRequest("ClientId header missing");

            try
            {
                switch (actionType)
                {
                    case 0: // Update Subject
                        return Ok(await _subjectService.UpdateSubject(value, clientId));

                    case 1: // Update Optional Subject
                        return Ok(await _subjectService.UpdateOptionalSubject(value, clientId));

                    case 2: // Update SubSubject
                        return Ok(await _subjectService.UpdateSubSubject(value, clientId));

                    default:
                        response.Message = "Invalid actionType";
                        return BadRequest(response);
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Status = -1;
                response.Error = ex.Message;
                Repository.Error.ErrorBLL.CreateErrorLog("SubjectController", "UpdateSubject", ex.ToString());
                return StatusCode(500, response);
            }
        }



        [HttpGet("subject-info")]
        public async Task<ActionResult<ResponseModel>> FetchSubjectInfo(int actionType, string? param)
        {
            var response = new ResponseModel { IsSuccess = true, Status = 0 };
            //var clientId = Request.Headers["X-Client-Id"].FirstOrDefault();
            var clientId = "client1";
            if (string.IsNullOrEmpty(clientId))
                return BadRequest("ClientId header missing");

            try
            {
                switch (actionType)
                {
                    case 0:
                        return Ok(await _subjectService.GetSubjectsByClassId(param, clientId));

                    case 1:
                        return Ok(await _subjectService.GetOptionalSubjectsByClassId(param, clientId));

                    case 2:
                        return Ok(await _subjectService.GetSubSubjectsBySubjectId(param, clientId));

                    default:
                        response.Message = "Invalid actionType";
                        return BadRequest(response);
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Status = -1;
                response.Error = ex.Message;
                Repository.Error.ErrorBLL.CreateErrorLog("SubjectController", "FetchSubjectInfo", ex.ToString());
                return StatusCode(500, response);
            }
        }


        [HttpDelete("subject-info")]
        public async Task<ActionResult<ResponseModel>> DeleteSubject(
     [FromQuery] int actionType,
     [FromQuery] string? param)
        {
            var response = new ResponseModel { IsSuccess = true, Status = 0 };
            var clientId = Request.Headers["X-Client-Id"].FirstOrDefault();

            if (string.IsNullOrEmpty(param))
                return BadRequest("param (SubjectId) is required");

            try
            {
                switch (actionType)
                {
                    case 0: // Delete Subject
                        return Ok(await _subjectService.DeleteSubject(param, clientId));

                    case 1: // Delete Optional Subject
                        return Ok(await _subjectService.DeleteOptionalSubject(param, clientId));

                    case 2: // Delete SubSubject
                        return Ok(await _subjectService.DeleteSubSubject(param, clientId));

                    default:
                        response.Message = "Invalid actionType";
                        return BadRequest(response);
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Status = -1;
                response.Error = ex.Message;
                Repository.Error.ErrorBLL.CreateErrorLog("SubjectController", "DeleteSubject", ex.ToString());
                return StatusCode(500, response);
            }
        }


    }
}
