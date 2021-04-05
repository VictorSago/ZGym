
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace ZGym.Core.Entities
{
    public class ApplicationUser : IdentityUser
    {
        // Add properties

        public ICollection<ApplicationUserGymClass> AttendedClasses { get; set; }
        
        
    }
}