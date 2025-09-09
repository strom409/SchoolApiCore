using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using Student.Repository;
using Student.Services.ClassMaster;
using System.Text.Json;

namespace Student.Services.ClassMaster
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClassMasterController : ControllerBase
    {
        private readonly IClassMasterService _classMasterService;

        public ClassMasterController(IClassMasterService classMasterService)
        {
            _classMasterService = classMasterService;
        }


        [HttpPost("add-info")]
        public async Task<ActionResult<ResponseModel>> AddInfo([FromQuery] int actionType, [FromBody] object request)
        {
            var response = new ResponseModel { IsSuccess = true, Status = 0, Message = "Issue at Controller Level!" };

            try
            {
                var clientId = Request.Headers["X-Client-Id"].FirstOrDefault();

                if (string.IsNullOrEmpty(clientId))
                    return BadRequest("ClientId header missing");

                switch (actionType)
                {
                    case 0: // Add Class
                        var classDto = JsonConvert.DeserializeObject<ClassDto>(request.ToString());
                        response = await _classMasterService.AddClass(classDto, clientId);
                        break;

                    case 1: // Add Section
                        var sectionDto = JsonConvert.DeserializeObject<SectionDto>(request.ToString());
                        response = await _classMasterService.AddSection(sectionDto, clientId);
                        break;

                    case 2: // Upgrade Class + Subjects + Sections with Duplicate Check
                        var upgradeDto = JsonConvert.DeserializeObject<UpgradeClassDto>(request.ToString());
                        response = await _classMasterService.UpgradeClassSubjectsSectionsAsync(upgradeDto,clientId);
                        break;

                    default:
                        response.IsSuccess = false;
                        response.Message = "Invalid actionType.";
                        return Ok(response);
                }
            }
            catch (Exception ex)
            {
                Student.Repository.Error.ErrorBLL.CreateErrorLog("ClassMasterController", "AddInfo", ex.ToString());
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "An unexpected error occurred.";
                response.Error = ex.Message;
            }

            return Ok(response);
        }


        [HttpGet("class-master")]
        public async Task<ActionResult<ResponseModel>> FetchClassMasterInfo(int actionType, string? param)
        {
            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0
            };

            var clientId = Request.Headers["X-Client-Id"].FirstOrDefault();
            if (string.IsNullOrEmpty(clientId))
                return BadRequest("ClientId header missing");

            try
            {
                switch (actionType)
                {
                    case 0:
                        return Ok(await _classMasterService.GetEducationalDepartments(clientId));

                    case 1:
                        if (!int.TryParse(param, out int classId))
                            return BadRequest(response);

                        return Ok(await _classMasterService.GetSectionsByClassId(classId, clientId));

                    case 2:
                        if (string.IsNullOrWhiteSpace(param))
                            return BadRequest(response);

                        return Ok(await _classMasterService.GetClassesBySessionWithDepartment(param, clientId));

                    default:
                        return BadRequest(response);
                }
            }
            catch (Exception ex)
            {
                Student.Repository.Error.ErrorBLL.CreateErrorLog("ClassMasterController", "FetchClassMasterInfo", ex.ToString());

                response.IsSuccess = false;
                response.Status = -1;
                response.Error = ex.Message;

                return StatusCode(500, response);
            }
        }


        [HttpPut("update-by-action")]
        public async Task<ActionResult<ResponseModel>> Update([FromQuery] int actionType, [FromBody] object dto)
        {
            var response = new ResponseModel { IsSuccess = false, Status = 0, Message = "Invalid request." };

            try
            {
                // You can later fetch this from headers if needed
                var clientId = Request.Headers["X-Client-Id"].FirstOrDefault();
                return BadRequest("ClientId header missing");

                switch (actionType)
                {
                    case 0: // Update Class
                        var classDto = JsonConvert.DeserializeObject<ClassDto>(dto.ToString());
                        if (classDto.ClassId <= 0)
                            return BadRequest(new ResponseModel { IsSuccess = false, Status = 0, Message = "Invalid ClassId for update." });

                        response = await _classMasterService.UpdateClass(classDto, clientId);
                        break;

                    case 1: // Update Section
                        var sectionDto = JsonConvert.DeserializeObject<SectionDto>(dto.ToString());
                        if (sectionDto.SectionID <= 0)
                            return BadRequest(new ResponseModel { IsSuccess = false, Status = 0, Message = "Invalid SectionID for update." });

                        response = await _classMasterService.UpdateSection(sectionDto, clientId);
                        break;

                    default:
                        response.Message = "Invalid actionType.";
                        break;
                }
            }
            catch (Exception ex)
            {
                Repository.Error.ErrorBLL.CreateErrorLog("ClassMasterController", "UpdateByActionType", ex.ToString());
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "An error occurred during update.";
                response.Error = ex.Message;
            }

            return StatusCode(200, response); // Always return 200 with proper response structure
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> Delete([FromQuery] int actionType, [FromQuery] int id)
        {
            var response = new ResponseModel { IsSuccess = true, Status = 0, Message = "Issue at Controller Level!" };

            try
            {
                var clientId = Request.Headers["X-Client-Id"].FirstOrDefault();
                if (string.IsNullOrEmpty(clientId))
                    return BadRequest("ClientId header missing.");

                switch (actionType)
                {
                    case 0: // Delete Class
                        response = await _classMasterService.DeleteClass(id, clientId);
                        break;

                    case 1: // Delete Section
                        response = await _classMasterService.DeleteSection(id, clientId);
                        break;

                    default:
                        response.IsSuccess = false;
                        response.Status = 0;
                        return BadRequest(response);
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error occurred during deletion.";
                response.Error = ex.Message;
                Repository.Error.ErrorBLL.CreateErrorLog("ClassMasterController", "DeleteByAction", ex.ToString());
            }

            return StatusCode(response.Status == 0 ? 400 : 200, response);
        }



    }
}


