using login.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Login.Services.HT
{
    [Route("api/[controller]")]
    [ApiController]
    public class HTController : ControllerBase
    {

        private readonly IHTService _htService;
        public HTController(IHTService htService)
        {
            _htService = htService;
        }

        [HttpGet]
        public async Task<ActionResult<ResponseModel>> GetHT([FromQuery] string clientId, [FromQuery] int actionType)
        {
            var response = new ResponseModel { IsSuccess = true, Status = 0, Message = "Invalid Request" };

            if (string.IsNullOrEmpty(clientId))
                return BadRequest("ClientId is required");

            try
            {
                switch (actionType)
                {
                    case 0:
                        return Ok(await _htService.getHT(clientId));

                    default:
                        return BadRequest(new ResponseModel
                        {
                            IsSuccess = false,
                            Message = "Invalid actionType"
                        });
                }
            }
            catch (Exception ex)
            {
                login.Repository.Error.ErrorBLL.CreateErrorLog("HTController", "GetHT", ex.ToString());

                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Exception occurred.";
                response.Error = ex.Message;

                return StatusCode(500, response);
            }
        }

        [HttpPut]
        public async Task<ActionResult<ResponseModel>> UpdateHT([FromQuery] int actionType, [FromQuery] string clientId,
         [FromBody] object payload)

        {
            var response = new ResponseModel { IsSuccess = true, Status = 0, Message = "Invalid Request" };

            // Validate clientId
            if (string.IsNullOrEmpty(clientId))
                return BadRequest("ClientId is required");
            try
            {
                switch (actionType)
                {
                    case 0:
                        var htDto = JsonConvert.DeserializeObject<HTModel>(payload.ToString());
                        if (htDto == null)
                            return BadRequest(new ResponseModel
                            {
                                Message = "Invalid payload for UpdateHT",
                            });

                        response = await _htService.UpdateHT(htDto, clientId);
                        break;

                    default:
                        return BadRequest(new ResponseModel
                        {
                            IsSuccess = false,
                            Message = "Invalid actionType"
                        });
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                login.Repository.Error.ErrorBLL.CreateErrorLog("HTController", "UpdateHT", ex.ToString());

                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Exception occurred.";
                response.Error = ex.Message;

                return StatusCode(500, response);
            }
        }


    }
}
