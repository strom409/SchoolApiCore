using ClassTest.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ClassTest.Services
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClassTestController : ControllerBase
    {

        private readonly IClassTestService _classTestService;

        public ClassTestController(IClassTestService classTestService)
        {
            _classTestService = classTestService;
        }


        [HttpPost("class-test")]
        public async Task<ActionResult<ResponseModel>> AddClassTest(
     [FromQuery] int actionType,
     [FromBody] List<ClassTestDTO> value)
        {
            var response = new ResponseModel { IsSuccess = true, Status = 0, Message = "Issue at Controller Level!" };
            //var clientId = Request.Headers["X-Client-Id"].FirstOrDefault();
            var clientId = "client1";
            if (string.IsNullOrEmpty(clientId))
                return BadRequest(new ResponseModel { IsSuccess = false, Error = "ClientId header missing." });

            try
            {
                if (value == null || value.Count == 0)
                {
                    return BadRequest(new ResponseModel { IsSuccess = false, Error = "Invalid payload." });
                }

                switch (actionType)
                {
                    case 0: // Add Class Test Max Marks
                        return Ok(await _classTestService.AddClassTestMaxMarks(value, clientId));

                    case 1: // Add Class Test Marks
                        return Ok(await _classTestService.AddClassTestMarks(value, clientId));

                    default:
                        return BadRequest(new ResponseModel { IsSuccess = false, Error = "Invalid ActionType." });
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Status = -1;
                response.Error = ex.Message;
                Repository.Error.ErrorBLL.CreateErrorLog("ClassTestController", "PostClassTest", ex.ToString());
                return StatusCode(500, response);
            }
        }


        [HttpPut("class-test/update")]
        public async Task<IActionResult> UpdateClassTest([FromQuery] int actionType, [FromBody] List<ClassTestDTO> value)
        {
            var response = new ResponseModel { IsSuccess = true, Status = 0, Message = "Issue at Controller Level!" };

            var clientId = Request.Headers["X-Client-Id"].FirstOrDefault();
            if (string.IsNullOrEmpty(clientId))
                return BadRequest(new ResponseModel { IsSuccess = false, Error = "ClientId header is missing." });

            try
            {
                if (value == null || value.Count == 0)
                    return BadRequest(new ResponseModel { IsSuccess = false, Error = "Request body is empty or invalid." });

                switch (actionType)
                {
                    case 0: // Update class test max marks
                        response = await _classTestService.UpdateClassTestMaxMarks(value, clientId);
                        break;

                    case 1: // Edit/update class test marks
                        response = await _classTestService.EditUpdateClassTestMarks(value, clientId);
                        break;

                    default:
                        response.Message = "Invalid ActionType.";
                        return BadRequest(response);
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Status = -1;
                response.Error = ex.Message;
                Repository.Error.ErrorBLL.CreateErrorLog("ClassTestController", "UpdateClassTest", ex.ToString());
                return StatusCode(500, response);
            }
        }



        [HttpGet("class-test")]
        public async Task<ActionResult<ResponseModel>> FetchClassTest(int actionType, string? param)
        {
            var response = new ResponseModel { IsSuccess = true, Status = 0 };
            var clientId = Request.Headers["X-Client-Id"].FirstOrDefault();

            if (string.IsNullOrEmpty(clientId))
                return BadRequest("ClientId header missing");

            try
            {
                switch (actionType)
                {
                    case 0:
                        return Ok(await _classTestService.GetSubjectForMaxMarks(param, clientId));

                    case 1:
                        return Ok(await _classTestService.GetMaxMarks(param, clientId));

                    case 2:
                        return Ok(await _classTestService.GetStudents(param, clientId));

                    case 3:
                        return Ok(await _classTestService.GetStudentsWithMarks(param, clientId));

                    case 4:
                        return Ok(await _classTestService.ViewDateWiseResult(param, clientId));

                    case 5:
                        return Ok(await _classTestService.ClassTestReport(param, clientId));

                    case 6:
                        return Ok(await _classTestService.ViewDateWiseResultForAllSubjects(param, clientId));

                    case 7:
                        return Ok(await _classTestService.ViewDateWiseResultForTotalMarks(param, clientId));

                    case 8:
                        return Ok(await _classTestService.ViewDateWiseTotalMMandObtMarks(param, clientId));

                    case 9:
                        return Ok(await _classTestService.GetMissingClassTestMarks(param, clientId));

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
                Repository.Error.ErrorBLL.CreateErrorLog("ClassTestController", "FetchClassTest", ex.ToString());
                return StatusCode(500, response);
            }
        }

    }

}

