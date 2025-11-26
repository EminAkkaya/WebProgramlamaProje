namespace WebProgramlamaProje.Models
{
    public class Appointment : BaseEntity
    {
        public DateTime AppointmentDate { get; set; } // Randevu Tarihi
        public TimeSpan StartTime { get; set; }       // Başlangıç Saati
        public TimeSpan EndTime { get; set; }         // Bitiş (Start + Service.Duration)

        public AppointmentStatus Status { get; set; }

        // İlişkiler
        public string MemberId { get; set; } // AppUser Id'si string gelir
        public AppUser Member { get; set; }

        public int TrainerId { get; set; }
        public Trainer Trainer { get; set; }

        public int ServiceId { get; set; }
        public Service Service { get; set; }
    }

    public enum AppointmentStatus
    {
        Pending,
        Confirmed,
        Cancelled,
        Completed
    }
}
