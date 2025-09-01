using Microsoft.AspNetCore.Mvc;
using Student.Repository;
using Student.Services.Students;

[Route("api/[controller]")]
[ApiController]
public class RPAlamStudentController : ControllerBase
{
    private readonly IRPAlamStudentService _rPAlamStudentService;

    public RPAlamStudentController(IRPAlamStudentService rPAlamStudentService)
    {
        _rPAlamStudentService = rPAlamStudentService;
    }

    [HttpGet("fetch-rp-alam-info/{actionType}/{param?}")]
    public async Task<ActionResult<ResponseModel>> FetchRPAlamStudentInfo(int actionType, string? param)
    {
        var response = new ResponseModel
        {
            IsSuccess = true,
            Status = 0,
            Message = "Issue at Controller Level!"
        };

        var clientId = User.Claims.FirstOrDefault(c => c.Type == "ClientId")?.Value;
        if (string.IsNullOrEmpty(clientId))
            return Unauthorized("ClientId claim missing");

        try
        {
            switch (actionType)
            {
                case 0:
                    response = await _rPAlamStudentService.GetAllStudentsOnStudentInfoIdAsync(param, clientId);
                    return Ok(response);
                case 1:
                    response = await _rPAlamStudentService.GetAttendanceMarkedOrNotAsPerDateAsync(param, clientId);
                    return Ok(response);
                case 2:
                    response = await _rPAlamStudentService.GetCopyCheckingOnSectionIdAsync(param, clientId);
                    return Ok(response);
                case 3:
                    response = await _rPAlamStudentService.GetPTMReportAsync(param, clientId);
                    return Ok(response);
                default:
                    response.Message = "Invalid Type Request ID";
                    return Ok(response);
            }
        }
        catch (Exception ex)
        {
            response.IsSuccess = false;
            response.Status = -1;
            response.Error = ex.Message;
            return StatusCode(500, response);
        }
    }
}
