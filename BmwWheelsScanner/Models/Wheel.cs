using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BmwWheelsScanner.Models
{
    public class Wheel 
    {
        public string BtnTabId { get; set; }
        public string WheelStyleName { get; set; }
        public string WheelPicture { get; set; }
        public bool MostPopular { get; set; }
    }
}
