using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpotsCheckService.Models
{
    public class ParkingSpotModel
    { 
        [JsonProperty("spot_index")]
        public int SpotIndex { get; set; }

        [JsonProperty("status")]
        public bool Status { get; set; }
        
    }
}
