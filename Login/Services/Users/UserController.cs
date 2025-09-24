using login.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Login.Services.Users
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

        [HttpPost("User")]
        public async Task<ActionResult<ResponseModel>> AddUser([FromQuery] int actionType, [FromForm] RequestUserDto request)
        {
            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "Issue at Controller Level !"
            };

            try
            {

                var clientId = Request.Headers["X-Client-Id"].FirstOrDefault();

                if (string.IsNullOrEmpty(clientId))
                    return BadRequest("ClientId header missing");
                switch (actionType)
                {
                    case 0: // Add
                        response = await _userService.AddUserAsync(request, clientId);
                        break;

                    default:
                        response.IsSuccess = false;
                        response.Status = -1;
                        response.Message = "Invalid actionType provided.";
                        break;
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error: " + ex.ToString();
                login.Repository.Error.ErrorBLL.CreateErrorLog("UserController", "AddUser",
                    ex.ToString()
                );

                return Ok(response);
            }
        }

        [HttpPut("UpdateUser")]
        public async Task<ActionResult<ResponseModel>> UpdateUser([FromQuery] int actionType, [FromForm] RequestUserDto request)
        {
            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "Issue at Controller Level !"
            };

            try
            {

                //  var clientId = Request.Headers["X-Client-Id"].FirstOrDefault();
                var clientId = "client1";

                if (string.IsNullOrEmpty(clientId))
                    return BadRequest("ClientId header missing");
                switch (actionType)
                {
                    case 0: // Add
                        response = await _userService.UpdateUserAsync(request, clientId);
                        break;
                    case 1: // Add
                        response = await _userService.ChangeUserPasswordAsync(request, clientId);
                        break;
                    default:
                        response.IsSuccess = false;
                        response.Status = -1;
                        response.Message = "Invalid actionType provided.";
                        break;
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error: " + ex.ToString();
                login.Repository.Error.ErrorBLL.CreateErrorLog("UserController", "UpdateUser",
                    ex.ToString()
                );

                return Ok(response);
            }
        }


        [HttpDelete("Users/{userId}")]
        public async Task<ActionResult<ResponseModel>> DeleteUser([FromQuery] int actionType, [FromRoute] int userId)
        {
            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "Issue at Controller Level !"
            };

            try
            {
                var clientId = Request.Headers["X-Client-Id"].FirstOrDefault();

                if (string.IsNullOrEmpty(clientId))
                    return BadRequest("ClientId header missing");

                switch (actionType)
                {
                    case 0: // Delete
                        response = await _userService.DeleteUserAsync(userId, clientId);
                        break;

                    default:
                        response.IsSuccess = false;
                        response.Status = -1;
                        response.Message = "Invalid actionType provided.";
                        break;
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error: " + ex.ToString();
                login.Repository.Error.ErrorBLL.CreateErrorLog("UserController", "DeleteUser",
                    ex.ToString()
                );

                return Ok(response);
            }
        }


    }
}
