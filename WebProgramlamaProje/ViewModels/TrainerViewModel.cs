using System.ComponentModel.DataAnnotations;
using WebProgramlamaProje.Models;

namespace WebProgramlamaProje.ViewModels
{
    public class TrainerViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Ad Soyad alanı zorunludur.")]
        [Display(Name = "Ad Soyad")]
        public string FullName { get; set; }

        [Display(Name = "Biyografi")]
        public string Bio { get; set; }

        [Display(Name = "Fotoğraf Linki")]
        public string PhotoUrl { get; set; }

        [Required]
        [Display(Name = "Mesai Başlangıç")]
        public TimeSpan ShiftStart { get; set; }

        [Required]
        [Display(Name = "Mesai Bitiş")]
        public TimeSpan ShiftEnd { get; set; }

        // --- Checkbox İşlemleri İçin ---

        // Kullanıcının formda seçtiği Hizmet ID'leri buraya gelecek
        public List<int> SelectedServiceIds { get; set; } = new List<int>();

        // View tarafında Checkbox'ları listelemek için tüm hizmetler
        // (Controller'da dolduracağız)
        public List<Service> AllServices { get; set; }
    }
}