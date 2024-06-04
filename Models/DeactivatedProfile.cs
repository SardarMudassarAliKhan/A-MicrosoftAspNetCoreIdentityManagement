using System.ComponentModel.DataAnnotations;

namespace A_MicrosoftAspNetCoreIdentityManagement.Models
{
    public class DeactivatedProfile
    {
        public int Id { get; set; }

        public int ProfileId { get; set; }

        [Required]
        public string Reason { get; set; }

        public DateTime DateTime { get; set; }
        public Profile Profile { get; set; }
    }
}
