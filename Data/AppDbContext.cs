using A_MicrosoftAspNetCoreIdentityManagement.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace A_MicrosoftAspNetCoreIdentityManagement.Data
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        public DbSet<Profile> Profiles { get; set; }
        public DbSet<ProfileBadge> ProfileBadges { get; set; }
        public DbSet<Badge> badges { get; set; }
        public DbSet<DeactivatedProfile> DeactivatedProfiles { get; set; }
    }
}
