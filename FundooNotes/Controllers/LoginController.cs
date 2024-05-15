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

        [HttpPost("updatePassword")]
        public async Task<IActionResult> ResetPassword(String email, String currentPassword, String newPassword)
        {
            try
            {
                await _login.UpdatePassword(email, currentPassword, newPassword);
                _logger.LogInformation("Password reset successful");
                var response = new ResponseStringModel
                {
                    Success = true,
                    Message = "Password Reset done"

                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("Password reset successful");
                var response = new ResponseDataModel<string>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                };
                return Ok(response);
            }
        }

        [HttpPost("forgetPassword")]
        public async Task<IActionResult> ForgotPassword(String Email)
        {
            try
            {
                await _login.ForgetPassword(Email);
                _logger.LogInformation("Email Sent");
                var response = new ResponseStringModel
                {
                    Success = true,
                    Message = "Email Sent Successfully"

                };
                return Ok(response);

            }
            catch (Exception ex)

            {
                _logger.LogError($"Error occured while sending mail {ex.Message}");
                var response = new ResponseStringModel
                {
                    Success = false,
                    Message = ex.Message,

                };
                return Ok(response);
            }
        }

        [HttpPatch("resetPassword")]
        public async Task<IActionResult> ResetPassword(String Token, String Password)
        {
            try
            {
                await _login.ResetPassword(Token, Password);
                _logger.LogInformation("Password has been reset");
                var response = new ResponseStringModel
                {
                    Success = true,
                    Message = "Password Reset Successful"
                };
                return Ok(response);
            } catch (Exception ex)
            {
                _logger.LogError($"{ex.Message}");
                var response = new ResponseStringModel
                {
                    Success = false,
                    Message = ex.Message,
                };
                return Ok(response);
            }
        }

    }
}
