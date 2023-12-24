using FirebaseService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirebaseService.Models
{
    public class ParkingLotModel
    {
        public string name { get; set; }

        public List<SpotsStatusesModel> spotsStatusDetail { get; set; }

        public int totalParkCount { get; set; }
        public int freeParkCount { get; set; }

        public Dictionary<string,object> prices { get; set; }

        public string adress { get; set; }

        public string mainPhoto { get; set; }
        public string firstFrame { get; set; }

    }
}
