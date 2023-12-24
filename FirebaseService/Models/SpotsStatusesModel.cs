using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirebaseService.Models
{
    [FirestoreData]
    public class SpotsStatusesModel
    {
        [FirestoreProperty]
        public string spotIndex { get; set; }

        [FirestoreProperty]
        public bool status { get; set; } // true : empty, false: occupied

        [FirestoreProperty]
        public bool isReserved { get; set; }
    }
}
