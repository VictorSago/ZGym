
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace ZGym.Core.Entities
{
    public class ApplicationUser : IdentityUser
    {
        // Add properties
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime TimeOfRegistration { get; set; }

        [Display(Name = "Full Name")] 
        public string FullName => $"{FirstName} {LastName}";

        public ICollection<ApplicationUserGymClass> AttendedClasses { get; set; }
        
        
    }
}