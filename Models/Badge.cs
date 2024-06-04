using System.ComponentModel.DataAnnotations;

namespace A_MicrosoftAspNetCoreIdentityManagement.Models
{
    public class Badge
    {
        public int BadgeId { get; set; }

        [Required]
        public string Title { get; set; }

        public string ImageUrl { get; set; }

        public string SmallImageUrl { get; set; }

        [Required]
        public string Description { get; set; }


        [Required]
        public string Type { get; set; }

        [Required]

        public int Condition { get; set; }
    }
}
