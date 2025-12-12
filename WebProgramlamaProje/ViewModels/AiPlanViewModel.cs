using System.ComponentModel.DataAnnotations;

namespace WebProgramlamaProje.ViewModels
{
    public class AiPlanViewModel
    {
        [Required(ErrorMessage = "Yaş bilgisi zorunludur.")]
        [Range(10, 100, ErrorMessage = "Geçerli bir yaş giriniz.")]
        public int Age { get; set; }

        [Required(ErrorMessage = "Boy bilgisi zorunludur (cm).")]
        [Range(100, 250, ErrorMessage = "Geçerli bir boy giriniz.")]
        public int Height { get; set; } // cm cinsinden

        [Required(ErrorMessage = "Kilo bilgisi zorunludur (kg).")]
        [Range(30, 300, ErrorMessage = "Geçerli bir kilo giriniz.")]
        public int Weight { get; set; } // kg cinsinden

        [Required(ErrorMessage = "Cinsiyet seçiniz.")]
        public string Gender { get; set; } // "Erkek", "Kadın"

        [Required(ErrorMessage = "Hedef seçiniz.")]
        public string Goal { get; set; } // "Kilo Vermek", "Kas Yapmak", "Formu Korumak"

        [Required(ErrorMessage = "Vücut tipi veya aktivite seviyesi seçiniz.")]
        public string ActivityLevel { get; set; } // "Hareketsiz", "Az Hareketli", "Sporcu"

        // Yapay zekadan dönen cevabı burada tutacağız
        public string? AiResponse { get; set; }
    }
}