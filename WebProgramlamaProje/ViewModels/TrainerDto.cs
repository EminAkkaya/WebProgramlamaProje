namespace WebProgramlamaProje.ViewModels
{
    public class TrainerDto
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Bio { get; set; }
        public string PhotoUrl { get; set; }
        public string WorkingHours { get; set; } // "09:00 - 18:00" gibi birleşik string
        public List<string> Specialties { get; set; } // Uzmanlık alanları (Hizmet isimleri)
    }
}