using HR.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HR.Services.Qualifications
{
    [Route("api/[controller]")]
    [ApiController]
    public class QualificationsController : ControllerBase
    {
            
        private  readonly IQualificationsService _qualificationsService;
        public QualificationsController(IQualificationsService qualificationsService)
        {
            _qualificationsService= qualificationsService;
        }

        [HttpGet("get-qualifications")]
        public async Task<ActionResult<ResponseModel>> GetQualifications([FromQuery] int actionType, string? param)
        {
            var response = new ResponseModel { IsSuccess = true, Status = 0, Message = "Issue at Controller Level !" };

            var clientId = Request.Headers["X-Client-Id"].FirstOrDefault() ?? "client1";
            if (string.IsNullOrEmpty(clientId))
                return BadRequest("ClientId header missing");


            try
            {
                switch (actionType)
                {
                    case 0:
                        response = await _qualificationsService.GetQualifications(clientId);
                        break;
                    case 1:
                        // Get qualification by ID
                        if (long.TryParse(param, out long id))
                        {
                            response = await _qualificationsService.GetQualificationById(param, clientId);
                        }
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
                Repository.Error.ErrorBLL.CreateErrorLog("QualificationController", "GetQualifications", ex.ToString());
            }

            return Ok(response);
        }

        [HttpPost("add-qualification")]
        public async Task<ActionResult<ResponseModel>> AddQualification([FromQuery] int actionType, [FromBody] QualificationModel value)
        {
            #region Initialize Response
            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "Qualification not added"
            };
            #endregion

            var clientId = Request.Headers["X-Client-Id"].FirstOrDefault() ?? "client1";
            if (string.IsNullOrEmpty(clientId))
                return BadRequest("ClientId header missing");


            try
            {
                #region Switch on actionType
                switch (actionType)
                {
                    case 0:
                        response = await _qualificationsService.AddQualification(value, clientId);
                        break;

                    default:
                        response.IsSuccess = false;
                        response.Message = "Invalid actionType";
                        break;
                }
                #endregion
            }
            catch (Exception ex)
            {
                #region Error Handling
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error: " + ex.Message;
                Repository.Error.ErrorBLL.CreateErrorLog("QualificationController", "AddQualification", ex.ToString());
                #endregion
            }

            return Ok(response);
        }


        [HttpPut("update-qualification")]
        public async Task<ActionResult<ResponseModel>> UpdateQualification([FromQuery] int actionType, [FromBody] QualificationModel value)
        {
            #region Initialize Response
            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "Qualification not updated"
            };
            #endregion

            var clientId = Request.Headers["X-Client-Id"].FirstOrDefault() ?? "client1";
            if (string.IsNullOrEmpty(clientId))
                return BadRequest("ClientId header missing");


            try
            {
                #region Switch on actionType
                switch (actionType)
                {
                    case 0: // Update
                        response = await _qualificationsService.UpdateQualification(value, clientId);
                        break;

                    default:
                        response.IsSuccess = false;
                        response.Message = "Invalid actionType";
                        break;
                }
                #endregion
            }
            catch (Exception ex)
            {
                #region Error Logging
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error: " + ex.Message;
                Repository.Error.ErrorBLL.CreateErrorLog("QualificationController", "UpdateQualification", ex.ToString());
                #endregion
            }

            return Ok(response);
        }



        [HttpDelete("delete-qualification")]
        public async Task<ActionResult<ResponseModel>> DeleteQualification([FromQuery] int actionType, [FromQuery] string id)
        {
            #region Initialize Response
            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "Qualification not deleted"
            };
            #endregion

            var clientId = Request.Headers["X-Client-Id"].FirstOrDefault() ?? "client1";
            if (string.IsNullOrEmpty(clientId))
                return BadRequest("ClientId header missing");



            try
            {
                #region Switch on actionType
                switch (actionType)
                {
                    case 0: // Delete
                        if (string.IsNullOrEmpty(id))
                        {
                            response.IsSuccess = false;
                            response.Status = -2;
                            response.Message = "Qualification ID is required for delete.";
                        }
                        else
                        {
                            response = await _qualificationsService.DeleteQualification(id, clientId);
                        }
                        break;

                    default:
                        response.IsSuccess = false;
                        response.Message = "Invalid actionType";
                        break;
                }
                #endregion
            }
            catch (Exception ex)
            {
                #region Error Logging
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error: " + ex.Message;
                Repository.Error.ErrorBLL.CreateErrorLog("QualificationController", "DeleteQualification", ex.ToString());
                #endregion
            }

            return Ok(response);
        }
    }
}
