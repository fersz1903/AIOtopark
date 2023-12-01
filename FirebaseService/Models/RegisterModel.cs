using System.ComponentModel.DataAnnotations;

namespace FirebaseService.Models
{
    public class RegisterModel
    {
        [Required(ErrorMessage ="Alan Boş Bırakılamaz!!")]
        public string fname { get; set;}

        [Required]
        [Phone]
        [MinLength(10,ErrorMessage ="Lütfen 5xxx şeklinde telefon numaranızı giriniz.")]
        public string phone { get; set; }

        [Required(ErrorMessage = "Alan Boş Bırakılamaz!!")]
        [EmailAddress]
        public string email { get; set; }
        [Required(ErrorMessage = "Alan Boş Bırakılamaz!!")]
        [MinLength(6,ErrorMessage ="Şifre En Az 6 Karakter olmalıdır!")]
        public string password { get; set; }
    }
}
