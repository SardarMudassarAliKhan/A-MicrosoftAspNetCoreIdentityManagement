using System.ComponentModel.DataAnnotations;

namespace A_MicrosoftAspNetCoreIdentityManagement.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Please enter your email")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Display(Name = "Password")]
        [Required(ErrorMessage = "Please enter your password.")]
        public string Password { get; set; }

        [Display(Name = "Remember Me")]
        public bool RememberMe { get; set; }
    }
}
