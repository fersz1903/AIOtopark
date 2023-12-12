using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirebaseService.Models
{
    public class UserModel
    {
        public string fname { get; set; }

        public string phone { get; set; }

        public string email { get; set; }
    }
}
