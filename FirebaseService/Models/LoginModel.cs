using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirebaseService.Models
{
    public class LoginModel
    {
        [Required]
        [EmailAddress]
        public string email { get; set; }

        [Required(ErrorMessage = "Alan Boş Bırakılamaz!!")]
        [MinLength(6, ErrorMessage = "Şifre En Az 6 Karakter olmalıdır!")]
        public string password { get; set; }
    }
}
