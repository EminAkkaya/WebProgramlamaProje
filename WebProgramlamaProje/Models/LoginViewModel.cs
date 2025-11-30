using System.ComponentModel.DataAnnotations;

namespace WebProgramlamaProje.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Kullanıcı adı gereklidir.")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Parola gereklidir.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Beni Hatırla")]
        public bool RememberMe { get; set; }
    }
}
