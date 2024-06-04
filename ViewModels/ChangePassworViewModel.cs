using System.ComponentModel.DataAnnotations;

namespace A_MicrosoftAspNetCoreIdentityManagement.ViewModels
{
    public class ChangePassworViewModel
    {
        [Display(Name = "Current Password")]
        [Required]
        public string CurrentPassword { get; set; }

        [Display(Name = "New Password")]
        [Required]
        public string NewPassword { get; set; }

        [Display(Name = "Confirm New Password")]
        [Required]
        [Compare("NewPassword")]
        public string ConfirmNewPassword { get; set; }
    }
}
