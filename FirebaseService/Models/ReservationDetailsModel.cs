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
        [FirestoreProperty]
        public string startDate { get; set; }
        [FirestoreProperty]
        public string startHour { get; set; }
        [FirestoreProperty]
        public string range { get; set; }
        [FirestoreProperty]
        public string plate { get; set; }
        [FirestoreProperty]
        public string reservatedSpot { get; set; }
        
    }
}
