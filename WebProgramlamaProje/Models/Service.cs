namespace WebProgramlamaProje.Models
{
    public class Service : BaseEntity
    {
        public string Name { get; set; } // Örn: "Birebir Pilates"

        public string Description { get; set; }
        public int DurationMinutes { get; set; } // Örn: 45 dk (Randevu süresi hesaplamak için kritik)
        public decimal Price { get; set; }

        public ICollection<TrainerService>? TrainerServices { get; set; }
    }
}
