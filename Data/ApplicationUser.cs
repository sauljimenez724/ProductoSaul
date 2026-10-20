using Microsoft.AspNetCore.Identity;

namespace ProductoSaul.Data
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;
        public string RolSeleccionado { get; set; } = string.Empty;
    }

}
