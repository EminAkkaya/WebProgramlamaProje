using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebProgramlamaProje.Data;
using WebProgramlamaProje.Models;
using WebProgramlamaProje.ViewModels;

namespace WebProgramlamaProje.Controllers
{
    [Authorize] // Sadece giriş yapmış kullanıcılar erişebilir
    public class AppointmentsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public AppointmentsController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Appointments
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            var appointmentsQuery = _context.Appointments
                .Include(a => a.Service)
                .Include(a => a.Trainer)
                .AsQueryable();

            if (!User.IsInRole("Admin"))
            {
                appointmentsQuery = appointmentsQuery.Where(a => a.MemberId == user.Id);
            }

            var appointments = await appointmentsQuery
                .OrderByDescending(a => a.AppointmentDate)
                .ThenBy(a => a.StartTime)
                .ToListAsync();

            return View(appointments);
        }

        // GET: Appointments/Create
        public async Task<IActionResult> Create()
        {
            var model = new AppointmentCreateViewModel
            {
                Services = await _context.Services.Where(s => !s.IsDeleted).ToListAsync(),
                Trainers = await _context.Trainers.Where(t => !t.IsDeleted).ToListAsync()
            };
            return View(model);
        }

        // POST: Appointments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AppointmentCreateViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);

            if (ModelState.IsValid)
            {
                // 1. Verileri Çekelim
                var service = await _context.Services.FindAsync(model.ServiceId);
                var trainer = await _context.Trainers.FindAsync(model.TrainerId);

                if (service == null || trainer == null)
                {
                    ModelState.AddModelError("", "Hizmet veya Antrenör bulunamadı.");
                    return ReloadView(model);
                }

                // 2. Bitiş Süresini Hesapla (TimeSpan olarak DB'ye kaydetmek için)
                TimeSpan calculatedEndTime = model.StartTime.Add(TimeSpan.FromMinutes(service.DurationMinutes));

                // --- 3. MESAİ KONTROLÜ (Gece Yarısı Mantığı Dahil) ---

                // Mesai saatlerini DateTime'a çevir
                DateTime shiftStartDt = model.AppointmentDate.Date + trainer.ShiftStart;
                DateTime shiftEndDt = model.AppointmentDate.Date + trainer.ShiftEnd;

                // Eğer vardiya gece yarısını geçiyorsa (Örn: 18:00 - 02:00) bitiş tarihini bir gün ötele
                if (trainer.ShiftEnd <= trainer.ShiftStart)
                {
                    shiftEndDt = shiftEndDt.AddDays(1);
                }

                // Talep edilen randevu saatlerini DateTime'a çevir
                DateTime appStartDt = model.AppointmentDate.Date + model.StartTime;
                DateTime appEndDt = appStartDt.AddMinutes(service.DurationMinutes);

                // Kontrol: Randevu, vardiya sınırları dışında mı?
                if (appStartDt < shiftStartDt || appEndDt > shiftEndDt)
                {
                    string startStr = trainer.ShiftStart.ToString(@"hh\:mm");
                    string endStr = trainer.ShiftEnd == TimeSpan.Zero ? "24:00" : trainer.ShiftEnd.ToString(@"hh\:mm");

                    ModelState.AddModelError("", $"Bu antrenör sadece {startStr} - {endStr} saatleri arasında hizmet vermektedir.");
                    return ReloadView(model);
                }

                // --- 4. ÇAKIŞMA KONTROLÜ (Conflict Check) ---

                bool isConflict = await _context.Appointments.AnyAsync(a =>
                    a.TrainerId == model.TrainerId &&
                    a.AppointmentDate.Date == model.AppointmentDate.Date && // Aynı gün
                    a.Status != AppointmentStatus.Cancelled && // İptal edilenler hariç
                    (model.StartTime < a.EndTime && calculatedEndTime > a.StartTime) // Zaman çakışması formülü
                );

                if (isConflict)
                {
                    ModelState.AddModelError("", "Seçilen saatte antrenörün başka bir randevusu mevcut. Lütfen başka bir saat seçiniz.");
                    return ReloadView(model);
                }

                // 5. Kayıt İşlemi
                var appointment = new Appointment
                {
                    MemberId = user.Id,
                    TrainerId = model.TrainerId,
                    ServiceId = model.ServiceId,
                    AppointmentDate = model.AppointmentDate,
                    StartTime = model.StartTime,
                    EndTime = calculatedEndTime, // Hesaplanan TimeSpan değeri
                    Status = AppointmentStatus.Pending,
                    CreatedDate = DateTime.Now
                };

                _context.Add(appointment);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return ReloadView(model);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PendingRequests()
        {
            // Sadece statüsü 'Pending' olanları getir
            var pendingAppointments = await _context.Appointments
                .Include(a => a.Member)   // Randevuyu alan üye (AppUser)
                .Include(a => a.Trainer)  // Antrenör
                .Include(a => a.Service)  // Hizmet
                .Where(a => a.Status == AppointmentStatus.Pending)
                .OrderBy(a => a.AppointmentDate)
                .ThenBy(a => a.StartTime)
                .ToListAsync();

            return View(pendingAppointments);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Approve(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null) return NotFound();

            // Durumu güncelle
            appointment.Status = AppointmentStatus.Confirmed;
            _context.Update(appointment);
            await _context.SaveChangesAsync();

            // Kullanıcıya mesaj göster (Layout'ta TempData kontrolü varsa görünür)
            TempData["SuccessMessage"] = "Randevu başarıyla onaylandı.";

            return RedirectToAction(nameof(PendingRequests));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Reject(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null) return NotFound();

            // Durumu güncelle
            appointment.Status = AppointmentStatus.Cancelled;
            _context.Update(appointment);
            await _context.SaveChangesAsync();

            TempData["ErrorMessage"] = "Randevu reddedildi/iptal edildi.";

            return RedirectToAction(nameof(PendingRequests));
        }

        private IActionResult ReloadView(AppointmentCreateViewModel model)
        {
            model.Services = _context.Services.Where(s => !s.IsDeleted).ToList();
            model.Trainers = _context.Trainers.Where(t => !t.IsDeleted).ToList();
            return View(model);
        }
    }
}