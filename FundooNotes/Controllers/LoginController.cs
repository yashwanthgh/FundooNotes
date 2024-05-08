using Microsoft.AspNetCore.Mvc;
using ModelLayer.RegistrationModel;
using ModelLayer.Response;
using BusinessLayer.Interfaces;

namespace FundooNotes.Controllers
{
    [Route("api/")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly ILoginBL _login;
        private readonly ILogger<LoginController> _logger;

        public LoginController(ILoginBL login, ILogger<LoginController> logger)
        {
            _login = login;
            _logger = logger;
        }

        [HttpPost("login")]
        public async Task<IActionResult> UserLogin(LoginUserModel loginUserModel)
        {
            try
            {
                // Authenticate the user and generate JWT token
                var Token = await _login.UserLogin(loginUserModel);
                var response = new ResponseDataModel<string>
                {
                    Success = true,
                    Message = "User Login Successfully",
                    Data = Token
                };
                return Ok(response);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to login {ex.Message}");
                var response = new ResponseStringModel
                {
                    Success = false,
                    Message = ex.Message,
                };
                return Ok(response );
            }
        }
    }
}
