using System.ComponentModel.DataAnnotations;

namespace WebProgramlamaProje.Models
{
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "Şifreler eşleşmiyor !")]
        public string ConfirmPassword { get; set; }
    }
}