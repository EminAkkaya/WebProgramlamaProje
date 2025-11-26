using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebProgramlamaProje.Models;

// Namespace'ini projene göre düzenlemelisin
namespace WebProgramlamaProje.Data
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // Veritabanı Tabloları (DbSet)
        public DbSet<Trainer> Trainers { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<TrainerService> TrainerServices { get; set; }
        public DbSet<Appointment> Appointments { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // Identity tablolarının (Users, Roles vb.) oluşması için bu satır ZORUNLUDUR.
            base.OnModelCreating(builder);

            // --- 1. Çoka-Çok İlişki (Trainer - Service) ---

            // Composite Key Tanımlaması (İki Id birleşip Primary Key olur)
            builder.Entity<TrainerService>()
                .HasKey(ts => new { ts.TrainerId, ts.ServiceId });

            // İlişki: Bir TrainerService, bir Trainer'a aittir.
            builder.Entity<TrainerService>()
                .HasOne(ts => ts.Trainer)
                .WithMany(t => t.TrainerServices)
                .HasForeignKey(ts => ts.TrainerId);

            // İlişki: Bir TrainerService, bir Service'e aittir.
            builder.Entity<TrainerService>()
                .HasOne(ts => ts.Service)
                .WithMany(s => s.TrainerServices)
                .HasForeignKey(ts => ts.ServiceId);

            // --- 2. Randevu (Appointment) İlişkileri ---

            // Member (AppUser) -> Appointment İlişkisi
            builder.Entity<Appointment>()
                .HasOne(a => a.Member)
                .WithMany(u => u.Appointments)
                .HasForeignKey(a => a.MemberId)
                .OnDelete(DeleteBehavior.Restrict); // Üye silinirse geçmiş randevuları silinmesin (veya hata versin)

            // Trainer -> Appointment İlişkisi
            builder.Entity<Appointment>()
                .HasOne(a => a.Trainer)
                .WithMany(t => t.Appointments)
                .HasForeignKey(a => a.TrainerId)
                .OnDelete(DeleteBehavior.Restrict); // Antrenör silinirse randevu kayıtları bozulmasın

            // Service -> Appointment İlişkisi
            // Service sınıfına "ICollection<Appointment>" eklemediysen sadece tek yönlü ilişki kurarız:
            builder.Entity<Appointment>()
                .HasOne(a => a.Service)
                .WithMany() // Service tarafında "Appointments" listesi yoksa boş bırakılır
                .HasForeignKey(a => a.ServiceId)
                .OnDelete(DeleteBehavior.Restrict);

            // --- 3. Hassas Veri Ayarları (Decimal Precision) ---
            // SQL Server'da decimal alanlar için hassasiyet belirtmek Best Practice'tir.
            builder.Entity<Service>()
                .Property(s => s.Price)
                .HasColumnType("decimal(18,2)");
        }
    }
}