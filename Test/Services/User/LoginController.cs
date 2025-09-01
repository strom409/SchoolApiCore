using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Test.Services.User;

namespace Student.Services.User
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        public readonly ILoginService _loginService;
        public LoginController(ILoginService loginService   )
        {
            _loginService = loginService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto request)
        {
            var result = await _loginService.LoginAsync(request);
            return Ok(result);
        }

        [HttpPost("CompareFaces")]
        public async Task<IActionResult> CompareFaces(CompareFacesRequestDTO request )
        {
           
            try
            {
                var result = await _loginService.CompareFacesAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

    }
}
