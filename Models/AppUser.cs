using Microsoft.AspNetCore.Identity;

namespace A_MicrosoftAspNetCoreIdentityManagement.Models
{
    public class AppUser : IdentityUser
    {
        [PersonalData]
        public string FirstName { get; set; }

        [PersonalData]
        public string LastName { get; set; }
    }
}
