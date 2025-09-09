using Examination_Management.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Examination_Management.Services.OptionalMaxMarks
{
    [Route("api/[controller]")]
    [ApiController]
    public class OptionalMaxMarksController : ControllerBase
    {
        private readonly IOptionalMaxMarksService _optionalMaxMarksService;
        public OptionalMaxMarksController(IOptionalMaxMarksService optionalMaxMarksService)
        {
            _optionalMaxMarksService = optionalMaxMarksService;
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
                    case 1: // Add OptionalMaxMarks
                        var addDto = JsonConvert.DeserializeObject<OptionalMaxMarksDto>(request.ToString());
                        response = await _optionalMaxMarksService.AddOptionalMaxMarks(addDto, clientId);
                        break;

                    default:
                        response.IsSuccess = false;
                        response.Message = "Invalid actionType provided.";
                        break;
                }
            }
            catch (Exception ex)
            {
                Repository.Error.ErrorBLL.CreateErrorLog("OptionalMaxMarksController", "Post", ex.ToString());
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
                    case 1: // Update OptionalMaxMarks
                        var updateDto = JsonConvert.DeserializeObject<OptionalMaxMarksDto>(request.ToString());
                        response = await _optionalMaxMarksService.UpdateOptionalMaxMarks(updateDto, clientId);
                        break;

                    default:
                        response.IsSuccess = false;
                        response.Message = "Invalid actionType provided.";
                        break;
                }
            }
            catch (Exception ex)
            {
                Repository.Error.ErrorBLL.CreateErrorLog("OptionalMaxMarksController", "Update", ex.ToString());
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

                    case 1: // Get OptionalMaxMarks by ID
                        response = await _optionalMaxMarksService.GetOptionalMaxMarksByFilter(param, clientId);
                        break;

                    default:
                        response.IsSuccess = false;
                        response.Message = "Invalid actionType provided.";
                        break;
                }
            }
            catch (Exception ex)
            {
                Repository.Error.ErrorBLL.CreateErrorLog("OptionalMaxMarksController", "Get", ex.ToString());
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "An unexpected error occurred.";
                response.Error = ex.Message;
            }

            return Ok(response);
        }

        //    [HttpDelete("delete")]
        //    public async Task<ActionResult<ResponseModel>> Delete([FromQuery] int actionType, [FromQuery] string param)
        //    {
        //        var response = new ResponseModel { IsSuccess = true, Status = 0, Message = "Issue at Controller Level!" };

        //        try
        //        {
        //            var clientId = Request.Headers["X-Client-Id"].FirstOrDefault() ?? "client1";

        //            switch (actionType)
        //            {
        //                case 1: // Soft delete OptionalMaxMarks by ID
        //                    response = await _optionalMaxMarksService.DeleteOptionalMaxMarks(param, clientId);
        //                    break;

        //                default:
        //                    response.IsSuccess = false;
        //                    response.Message = "Invalid actionType provided.";
        //                    break;
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            Repository.Error.ErrorBLL.CreateErrorLog("OptionalMaxMarksController", "Delete", ex.ToString());
        //            response.IsSuccess = false;
        //            response.Status = -1;
        //            response.Message = "An unexpected error occurred.";
        //            response.Error = ex.Message;
        //        }

        //        return Ok(response);
        //    }
        //}

    }
}
    

