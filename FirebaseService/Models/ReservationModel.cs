using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirebaseService.Models
{
    public class ReservationModel
    {
        public string userId { get; set; }
        public string startDate { get; set; }
        public int startHour { get; set; }
        public int range { get; set; }
        //public string endDate { get; set; }
        public string plate { get; set;}
    }
}
