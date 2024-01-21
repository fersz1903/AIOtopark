using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirebaseService.Models
{
    [FirestoreData]
    public class ReservationModel
    {
        [FirestoreProperty]
        public string userId { get; set; }
        [FirestoreProperty]
        public ReservationDetailsModel details { get; set; } = new ReservationDetailsModel();
    }
}
