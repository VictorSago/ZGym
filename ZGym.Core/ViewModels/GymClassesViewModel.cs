
using System;
using System.ComponentModel.DataAnnotations;

namespace ZGym.Core.ViewModels
{
    public class GymClassesViewModel
    {
        public int Id { get; set; }
        
        [Display(Name = "Pass Name")]
        public string Name { get; set; }

        [Display(Name = "Start Time")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd, HH:mm}", ApplyFormatInEditMode = false)]
        public DateTime StartTime { get; set; }

        [DisplayFormat(DataFormatString = "{0:hh\\:mm}", ApplyFormatInEditMode = true)]
        public TimeSpan Duration { get; set; }

        // Delete?
        public string Description { get; set; }
        
        // Delete?
        [Display(Name = "End Time")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd, HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime EndTime { get { return StartTime + Duration; } }

        public bool Attending { get; set; }
        
    }
}