using Microsoft.AspNetCore.Identity;

namespace WebProgramlamaProje.Models
{
    public class AppUser :IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime? DateOfBirth { get; set; }

        public double? Weight { get; set; }
        public double? Height { get; set; }
        public string? Gender { get; set; }

        // Kullanıcının randevuları
        public ICollection<Appointment>? Appointments { get; set; }
    }
}
