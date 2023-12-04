using FirebaseService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirebaseService.Models
{
    public class ParkingLotPreviewModel
    {
        public string name { get; set; }
        public int totalParkCount { get; set; }
        public int freeParkCount { get; set; }
        public string mainPhoto { get; set; }
    }
}
