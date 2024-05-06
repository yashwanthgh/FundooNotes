using BusinessLayer.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.RegistrationModel;

namespace FundooNotes.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegisterController : ControllerBase
    {
        private readonly IRegisterBL _register;
        private readonly Logger<RegisterController> _logger;

        public RegisterController(IRegisterBL register, Logger<RegisterController> logger)
        {
            _register = register;
            _logger = logger;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> UserRegistration(RegisterUserModel registerUserModel)
        {
            try
            {
                await _register.RegisterUser(registerUserModel);
                _logger.LogInformation("User Registration Successful");
                var response = new ResponseModel<RegisterUserModel>
                {
                    Success = true,
                    Message = "User Registration Successfull!",
                    /*
                    Data = new RegisterUserModel
                    {
                        FirstName = registerUserModel.FirstName,
                        LastName = registerUserModel.LastName,
                        Email = registerUserModel.Email,
                        Password = "* * * *"
                    }
                    */
                };
                return Ok(response);
            } catch (Exception ex)
            {
                _logger.LogError("Registration unsuccessful");
                var response = new ResponseModel<RegisterUserModel>
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
