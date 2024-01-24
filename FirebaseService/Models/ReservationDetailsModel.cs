using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirebaseService.Models
{
    [FirestoreData]
    public class ReservationDetailsModel
    {
        public string plName { get; set; }
        public string startDate { get; set; }
        public string startHour { get; set; }
        public string range { get; set; }
        public string plate { get; set; }
        public string reservatedSpot { get; set; }
        
    }
}
