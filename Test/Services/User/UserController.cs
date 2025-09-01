using Microsoft.AspNetCore.Mvc;
using Student.Services.User;
using System.Linq;

namespace Microservice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddUser([FromBody] RequestUserDto user)
        {
            var clientId = Request.Headers["X-Client-Id"].FirstOrDefault();

            if (string.IsNullOrEmpty(clientId))
                return BadRequest("ClientId header missing");

            var result = await _userService.AddUserAsync(user, clientId);

            return Ok(result);
        }
    }
}
