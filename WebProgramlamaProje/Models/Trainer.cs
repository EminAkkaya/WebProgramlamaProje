namespace WebProgramlamaProje.Models
{
    public class Trainer : BaseEntity
    {
        public string FullName { get; set; }
        public string Bio { get; set; } // Uzmanlık alanları açıklaması
        public string PhotoUrl { get; set; } // Arayüzde göstermek için

        public TimeSpan ShiftStart { get; set; }
        public TimeSpan ShiftEnd { get; set; }

        // Antrenörün verebildiği hizmetler
        public ICollection<TrainerService> TrainerServices { get; set; }
        public ICollection<Appointment> Appointments { get; set; }
    }
}
