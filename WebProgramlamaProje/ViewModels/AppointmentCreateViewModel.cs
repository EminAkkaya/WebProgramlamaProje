using System.ComponentModel.DataAnnotations;
using WebProgramlamaProje.Models;

namespace WebProgramlamaProje.ViewModels
{
    public class AppointmentCreateViewModel
    {
        [Required(ErrorMessage = "Lütfen bir hizmet seçiniz.")]
        [Display(Name = "Hizmet")]
        public int ServiceId { get; set; }

        [Required(ErrorMessage = "Lütfen bir antrenör seçiniz.")]
        [Display(Name = "Antrenör")]
        public int TrainerId { get; set; }

        [Required(ErrorMessage = "Tarih seçimi zorunludur.")]
        [DataType(DataType.Date)]
        [Display(Name = "Randevu Tarihi")]
        // Varsayılan olarak bugünün tarihini atayalım
        public DateTime AppointmentDate { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "Saat seçimi zorunludur.")]
        [DataType(DataType.Time)]
        [Display(Name = "Başlangıç Saati")]
        public TimeSpan StartTime { get; set; }

        // Dropdownları doldurmak için listeler
        public List<Service>? Services { get; set; }
        public List<Trainer>? Trainers { get; set; }
    }
}