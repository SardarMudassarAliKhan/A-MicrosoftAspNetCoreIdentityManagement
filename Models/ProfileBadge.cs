namespace A_MicrosoftAspNetCoreIdentityManagement.Models
{
    public class ProfileBadge
    {
        public int ProfileBadgeId { get; set; }
        public int BadgeId { get; set; }
        public int ProfileId { get; set; }
        public virtual Badge Badge { get; set; }
        public virtual Profile Profile { get; set; }
    }
}
