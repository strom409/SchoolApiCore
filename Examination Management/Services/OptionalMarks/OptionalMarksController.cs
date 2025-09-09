using Examination_Management.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Examination_Management.Services.OptionalMarks
{
    [Route("api/[controller]")]
    [ApiController]
    public class OptionalMarksController : ControllerBase
    {
        private readonly IOptionalMarksService _optionalMarksService;
        public OptionalMarksController(IOptionalMarksService optionalMarksService)
        {
            _optionalMarksService= optionalMarksService;
        }


        [HttpPost("post")]
        public async Task<ActionResult<ResponseModel>> Post([FromQuery] int actionType, [FromBody] object request)
        {
            var response = new ResponseModel { IsSuccess = true, Status = 0, Message = "Issue at Controller Level!" };

            try
            {
                var clientId = Request.Headers["X-Client-Id"].FirstOrDefault();
                if (string.IsNullOrEmpty(clientId))
                    return BadRequest("ClientId header missing");
                switch (actionType)
                {
                    case 1: // Add OptionalMarks
                        var addDto = JsonConvert.DeserializeObject<OptionalMarksDto>(request.ToString());
                        response = await _optionalMarksService.AddOptionalMark(addDto, clientId);
                        break;

                    default:
                        response.IsSuccess = false;
                        response.Message = "Invalid actionType provided.";
                        break;
                }
            }
            catch (Exception ex)
            {
                Repository.Error.ErrorBLL.CreateErrorLog("OptionalMarksController", "Post", ex.ToString());
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "An unexpected error occurred.";
                response.Error = ex.Message;
            }

            return Ok(response);
        }

        [HttpPut("update")]
        public async Task<ActionResult<ResponseModel>> Update([FromQuery] int actionType, [FromBody] object request)
        {
            var response = new ResponseModel { IsSuccess = true, Status = 0, Message = "Issue at Controller Level!" };

            try
            {
                var clientId = Request.Headers["X-Client-Id"].FirstOrDefault();
                if (string.IsNullOrEmpty(clientId))
                    return BadRequest("ClientId header missing");

                switch (actionType)
                {
                    case 1: // Update OptionalMarks
                        var updateDto = JsonConvert.DeserializeObject<OptionalMarksDto>(request.ToString());
                        response = await _optionalMarksService.UpdateOptionalMarks(updateDto, clientId);
                        break;

                    default:
                        response.IsSuccess = false;
                        response.Message = "Invalid actionType provided.";
                        break;
                }
            }
            catch (Exception ex)
            {
                Repository.Error.ErrorBLL.CreateErrorLog("OptionalMarksController", "Update", ex.ToString());
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "An unexpected error occurred.";
                response.Error = ex.Message;
            }

            return Ok(response);
        }

        [HttpGet("fetch")]
        public async Task<ActionResult<ResponseModel>> Get([FromQuery] int actionType, [FromQuery] string? param = null)
        {
            var response = new ResponseModel { IsSuccess = true, Status = 0, Message = "Issue at Controller Level!" };

            try
            {
                var clientId = Request.Headers["X-Client-Id"].FirstOrDefault();
                if (string.IsNullOrEmpty(clientId))
                    return BadRequest("ClientId header missing");

                switch (actionType)
                {
                  

                    case 1: // Get OptionalMarks by ID
                        response = await _optionalMarksService.GetMaxMarksByClassSectionSubjectUnit(param, clientId);
                        break;

                    default:
                        response.IsSuccess = false;
                        response.Message = "Invalid actionType provided.";
                        break;
                }
            }
            catch (Exception ex)
            {
                Repository.Error.ErrorBLL.CreateErrorLog("OptionalMarksController", "Get", ex.ToString());
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "An unexpected error occurred.";
                response.Error = ex.Message;
            }

            return Ok(response);
        }

        [HttpDelete("delete")]
        public async Task<ActionResult<ResponseModel>> Delete([FromQuery] int actionType, [FromQuery] string param)
        {
            var response = new ResponseModel { IsSuccess = true, Status = 0, Message = "Issue at Controller Level!" };

            try
            {
                var clientId = Request.Headers["X-Client-Id"].FirstOrDefault();
                if (string.IsNullOrEmpty(clientId))
                    return BadRequest("ClientId header missing");

                switch (actionType)
                {
                    case 1: // Delete OptionalMarks by ID
                        response = await _optionalMarksService.DeleteOptionalMarks(param, clientId);
                        break;

                    default:
                        response.IsSuccess = false;
                        response.Message = "Invalid actionType provided.";
                        break;
                }
            }
            catch (Exception ex)
            {
                Repository.Error.ErrorBLL.CreateErrorLog("OptionalMarksController", "Delete", ex.ToString());
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "An unexpected error occurred.";
                response.Error = ex.Message;
            }

            return Ok(response);
        }
    }
}
    

