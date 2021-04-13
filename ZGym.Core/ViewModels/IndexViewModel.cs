using System.Collections.Generic;

namespace ZGym.Core.ViewModels
{
    public class IndexViewModel
    {
        public IEnumerable<GymClassesViewModel> GymClasses { get; set; }

        public bool ShowHistory { get; set; }
        
    }
}
