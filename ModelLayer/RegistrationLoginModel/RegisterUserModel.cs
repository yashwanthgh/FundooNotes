
using System.ComponentModel.DataAnnotations;

namespace ModelLayer.RegistrationModel
{
    public class RegisterUserModel
    {
        [Required(ErrorMessage = "First Name required")]
        public string? FirstName { get; set; }

        [Required(ErrorMessage ="Last Name required")]
        public string? LastName { get; set; }

        [Required(ErrorMessage = "Email required")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Password required")]
        public string? Password { get; set; }
    }
}
