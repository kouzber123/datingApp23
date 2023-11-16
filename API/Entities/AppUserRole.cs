using Microsoft.AspNetCore.Identity;

namespace API.Entities
{

    /// <summary>
    /// connection table between app user and app roles
    /// </summary>
    public class AppUserRole : IdentityUserRole<int>
    {
        public AppUser User { get; set; }
        public AppRole Role { get; set; }
    }
}
