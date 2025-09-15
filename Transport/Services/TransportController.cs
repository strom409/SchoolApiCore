using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IO.Compression;
using Transport.Repository;

namespace Transport.Services
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransportController : ControllerBase
    {
        public readonly ITransportService _transportService;
        public TransportController(ITransportService transportService)
        {
            _transportService = transportService;
        }


        [HttpPost("AddTransport")]
        public async Task<ActionResult<ResponseModel>> AddTransportAsync(
    [FromQuery] int actionType,
    [FromBody] TransportDTO request)
        {
            var response = new ResponseModel { IsSuccess = true, Status = 0, Message = "Invalid Request!" };

            try
            {
                // Extract clientId from header (or from JWT claims if applicable)
                var clientId = Request.Headers["X-Client-Id"].FirstOrDefault();
                if (string.IsNullOrEmpty(clientId))
                    return BadRequest("ClientId header missing");

                // Validate input
                if (request == null)
                {
                    response.Message = "Invalid transport data.";
                    return BadRequest(response);
                }

                // Switch based on actionType
                switch (actionType)
                {
                    case 0: // Add Transport
                        return Ok(await _transportService.AddTransport(request, clientId));

                    case 1: // Add Bus Stops
                        return Ok(await _transportService.AddBusStops(request, clientId));

                    default:
                        response.Message = "Invalid actionType.";
                        return BadRequest(response);
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Exception occurred.";
                response.Error = ex.Message;

                return StatusCode(500, response);
            }
        }


        [HttpGet("FetchTransport")]
        public async Task<ActionResult<ResponseModel>> FetchTransport([FromQuery] int actionType, [FromQuery] string? param)
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
                        return Ok(await _transportService.GetTransportListOnSession(param, clientId));

                    case 1:
                        return Ok(await _transportService.GetTransportList(param, clientId));// GET BUS RATE FROM BUS STOPS

                    case 2:
                        return Ok(await _transportService.GetTransportListRateFromInfo(param, clientId));//BUS RATE FROM STUDENTINFO

                    case 3:
                        return Ok(await _transportService.GetTransportListWithBusRate(param, clientId));//NOT USED

                    case 4:
                        return Ok(await _transportService.GetTransportByRouteId(param, clientId));

                    case 5:
                        return Ok(await _transportService.GetStudentRouteDetails(param, clientId));//Not Used

                    case 6:
                        return Ok(await _transportService.GetStopListByName(param, clientId));

                    case 7:
                        return Ok(await _transportService.GetAllStops(clientId));

                    case 8:
                        return Ok(await _transportService.GetClassIdsAssigned(param, clientId));
                        //user

                    case 9:
                        return Ok(await _transportService.GetAssignedSections(param, clientId));
                        //sections

                    case 10:
                        return Ok(await _transportService.GetStudentBusReportListOnSectionID(param, clientId));

                    case 11:
                        return Ok(await _transportService.GetStudentListOnRouteID(param, clientId));//not used

                    case 12:
                        return Ok(await _transportService.GetStudentBusRateClasswise(param, clientId));

                    case 13:
                        return Ok(await _transportService.GetStudentBusRate(param, clientId));
                        
                    case 14:

                        return Ok(await _transportService.GetTransportNameById(param, clientId));

                    case 15:
                        return Ok(await _transportService.getTransportList(clientId));//not used

                    case 16:
                        return Ok(await _transportService.GetStopListWithLatLong(param, clientId));

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
                Repository.Error.ErrorBLL.CreateErrorLog("TransportController", "FetchTransport", ex.ToString());

                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Exception occurred.";
                response.Error = ex.Message;

                return StatusCode(500, response);
            }
        }


        [HttpPut("UpdateTransport")]
        public async Task<ActionResult<ResponseModel>> PutTransport(
     [FromQuery] int actionType,
     [FromBody] object payload) // we'll cast manually based on actionType
        {
            var response = new ResponseModel { IsSuccess = true, Status = 0, Message = "Invalid Request" };

            // Simulate clientId fetch
            var clientId = Request.Headers["X-Client-Id"].FirstOrDefault();
            if (string.IsNullOrEmpty(clientId))
                return BadRequest("ClientId header missing");

            try
            {
                switch (actionType)
                {
                    case 0:
                        // Update student's route and stop (TransportDTO)
                        var studentRouteDto = JsonConvert.DeserializeObject<TransportDTO>(payload.ToString());
                        return Ok(await _transportService.updateroute(studentRouteDto, clientId));

                    case 1:
                        // Update transport info (TransportDTO)
                        var transportDto = JsonConvert.DeserializeObject<TransportDTO>(payload.ToString());
                        return Ok(await _transportService.UpdateTransport(transportDto, clientId));

                    case 2:
                        // Update bus stop info (TransportDTO)
                        var stopDto = JsonConvert.DeserializeObject<TransportDTO>(payload.ToString());
                        return Ok(await _transportService.UpdateBusStops(stopDto, clientId));

                    case 3:
                        // Update bus stop lat/long (TransportDTO)
                        var latLongDto = JsonConvert.DeserializeObject<TransportDTO>(payload.ToString());
                        return Ok(await _transportService.UpdateBusStopsLatLong(latLongDto, clientId));

                    case 4:
                        // Update bus stop rates (StudentBusReportDTO)
                        var rateDto = JsonConvert.DeserializeObject<StudentBusReportDTO>(payload.ToString());
                        return Ok(await _transportService.UpdateBusStopRates(rateDto, clientId));

                    case 5:
                        // Update student route and bus stop (StudentBusReportDTO)
                        var fullDto = JsonConvert.DeserializeObject<StudentBusReportDTO>(payload.ToString());
                        return Ok(await _transportService.UpdateStudentRouteAndBusStop(fullDto, clientId));
                    case 6:
                        // Update aupdateBusStop 
                        var busDto = JsonConvert.DeserializeObject<BusStop>(payload.ToString());
                        return Ok(await _transportService.aupdateBusStop(busDto, clientId));
                    default:
                        response.Message = "Invalid actionType";
                        return BadRequest(response);
                }
            }
            catch (Exception ex)
            {
                Repository.Error.ErrorBLL.CreateErrorLog("TransportController", "PutTransport", ex.ToString());

                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Exception occurred";
                response.Error = ex.Message;

                return StatusCode(500, response);
            }
        }

        [HttpDelete("DeleteTransport")]
        public async Task<IActionResult> DeleteTransport(
    [FromQuery] int actionType,
    [FromQuery] string? param)
        {
            var response = new ResponseModel { IsSuccess = true, Status = 0, Message = "Invalid Request" };

            var clientId = Request.Headers["X-Client-Id"].FirstOrDefault();
            if (string.IsNullOrEmpty(clientId))
                return BadRequest("ClientId header missing");

            try
            {
                switch (actionType)
                {
                    case 0: // Delete Transport by RouteID
                       
                            return Ok(await _transportService.deleteTransport(param, clientId));
                    case 1: // Soft delete BusStop by BusStopID
                        return Ok(await _transportService.DeleteBusStop(param, clientId));

                    default:
                        response.Message = "Invalid actionType";
                        return BadRequest(response);
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Exception occurred";
                response.Error = ex.Message;
                return StatusCode(500, response);
            }
        }

    }
}
