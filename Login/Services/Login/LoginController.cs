using login.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Login.Services.Login
{
    [ApiController]
    public class LoginController : ControllerBase
    {
        public readonly ILoginService _loginService;
        public LoginController(ILoginService loginService)
        {
            _loginService = loginService;
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto request)
        {
            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "Issue at Controller Level !"
            };

            try
            {
                response = await _loginService.LoginAsync(request);
            }
            catch (Exception ex)
            {
                // Log the error
                login.Repository.Error.ErrorBLL.CreateErrorLog("LoginController", "Login", ex.ToString());

                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Exception occurred.";
                response.Error = ex.Message;
            }

            // Always return structured response
            return Ok(response);
        }


        //[HttpPost("login")]
        //public async Task<IActionResult> Login([FromBody] LoginDto request)
        //{
        //    var result = await _loginService.LoginAsync(request);
        //    return Ok(result);
        //}
        [HttpPost("validate-token")]
        public async Task<IActionResult> ValidateToken([FromBody] TokenValidationRequest request)
        {
            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "Issue at Controller Level !"
            };

            try
            {
                response = await _loginService.ValidateToken(request.Token, request.ClientId);
            }
            catch (Exception ex)
            {
                // Log the error
                login.Repository.Error.ErrorBLL.CreateErrorLog("LoginController", "ValidateToken", ex.ToString());

                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Exception occurred.";
                response.Error = ex.Message;
            }

            // Always return structured response
            return Ok(response);
        }



    }
}
