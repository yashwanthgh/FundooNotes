using BusinessLayer.Interfaces;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.RegistrationModel;
using ModelLayer.Response;
using NLog;
namespace FundooNotes.Controllers
{
    [Route("api/")]
    [ApiController]
    public class RegisterController : ControllerBase
    {
        private readonly IRegisterBL _register;
        private readonly ILogger<RegisterController> _logger;

        public RegisterController(IRegisterBL register, ILogger<RegisterController> logger)
        {
            _register = register;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<IActionResult> UserRegistration(RegisterUserModel registerUserModel)
        {
            try
            {
                await _register.RegisterUser(registerUserModel);
                _logger.LogInformation("User Registration Successful");

                var response = new ResponseStringModel
                {
                    Success = true,
                    Message = "User Registration Successful!",
                    /*
                    Data = new RegisterUserModel
                    {
                        FirstName = registerUserModel.FirstName,
                        LastName = registerUserModel.LastName,
                        Email = registerUserModel.Email,
                        Password = registerUserModel.Password
                    }
                    */
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("Registration unsuccessful");
                var response = new ResponseDataModel<RegisterUserModel>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                };
                return Ok(response);
            }
        }
    }
}
