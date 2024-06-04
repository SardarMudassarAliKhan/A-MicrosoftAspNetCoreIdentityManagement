using System.ComponentModel.DataAnnotations;

namespace A_MicrosoftAspNetCoreIdentityManagement.ViewModels
{
    public class VerifyContactViewModel
    {
        public string Email { get; set; }

        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }
        public string Status { get; set; }

        [Display(Name = "Verification Code")]
        [Required]
        public string VerificationCode { get; set; }
    }
}
