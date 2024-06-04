namespace A_MicrosoftAspNetCoreIdentityManagement.Models
{
        public class Profile
        {
            public int ProfileId { get; set; }
            public string? FirstName { get; set; }
            public string? LastName { get; set; }
            public string? FullName { get; set; }

            public string? TagLine { get; set; }
            public string? About { get; set; }
            public string? Country { get; set; }
            public string? City { get; set; }

            public string? UserName { get; set; }
            public string? PhoneNumber { get; set; }

            public string? FacebookLink { get; set; }
            public string? TwitterLink { get; set; }
            public DateTime MemberSince { get; set; }
            public string? Website { get; set; }
            public string? ContactVerified { get; set; }

            public string? HeaderImageUrl { get; set; }
            public string? DisplayImageUrl { get; set; }
            public string? ProfileUrl { get; set; }

            public int ProfileBadgeId { get; set; }
            //Navigational Properties
            public virtual ICollection<ProfileBadge>? ProfileBadges { get; set; }
    }
}
