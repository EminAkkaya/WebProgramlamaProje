using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebProgramlamaProje.Data;
using WebProgramlamaProje.Models;
using WebProgramlamaProje.ViewModels;

namespace WebProgramlamaProje.Controllers
{
    public class TrainersController : Controller
    {
        private readonly AppDbContext _context;

        public TrainersController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Trainers
        public async Task<IActionResult> Index()
        {
            // Sadece silinmemiş (IsDeleted == false) antrenörleri getiriyoruz.
            // İlişkili hizmetleri de (Include) çekiyoruz ki ekranda gösterebilelim.
            var trainers = await _context.Trainers
                .Where(t => !t.IsDeleted)
                .Include(t => t.TrainerServices)
                .ThenInclude(ts => ts.Service)
                .ToListAsync();

            return View(trainers);
        }

        // GET: Trainers/Create
        public async Task<IActionResult> Create()
        {
            // Form yüklenirken tüm hizmetleri çekip ViewModel'e koyuyoruz
            var model = new TrainerViewModel
            {
                AllServices = await _context.Services.Where(s => !s.IsDeleted).ToListAsync()
            };
            return View(model);
        }

        // POST: Trainers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TrainerViewModel model)
        {
            if (ModelState.IsValid)
            {
                // 1. ViewModel'den Entity'e çevir
                var trainer = new Trainer
                {
                    FullName = model.FullName,
                    Bio = model.Bio,
                    PhotoUrl = model.PhotoUrl,
                    ShiftStart = model.ShiftStart,
                    ShiftEnd = model.ShiftEnd,
                    CreatedDate = DateTime.Now
                };

                _context.Add(trainer);
                await _context.SaveChangesAsync(); // Önce kaydet ki TrainerId oluşsun

                // 2. Seçilen Hizmetleri TrainerServices tablosuna ekle
                if (model.SelectedServiceIds != null && model.SelectedServiceIds.Any())
                {
                    foreach (var serviceId in model.SelectedServiceIds)
                    {
                        var trainerService = new TrainerService
                        {
                            TrainerId = trainer.Id,
                            ServiceId = serviceId
                        };
                        _context.TrainerServices.Add(trainerService);
                    }
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }

            // Hata varsa listeyi tekrar doldurup sayfayı geri döndür
            model.AllServices = await _context.Services.Where(s => !s.IsDeleted).ToListAsync();
            return View(model);
        }

        // GET: Trainers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            // Antrenörü ve mevcut hizmetlerini çek
            var trainer = await _context.Trainers
                .Include(t => t.TrainerServices)
                .FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted);

            if (trainer == null) return NotFound();

            // Entity -> ViewModel çevrimi
            var model = new TrainerViewModel
            {
                Id = trainer.Id,
                FullName = trainer.FullName,
                Bio = trainer.Bio,
                PhotoUrl = trainer.PhotoUrl,
                ShiftStart = trainer.ShiftStart,
                ShiftEnd = trainer.ShiftEnd,
                // Mevcut seçili hizmet ID'lerini işaretle
                SelectedServiceIds = trainer.TrainerServices.Select(ts => ts.ServiceId).ToList(),
                // Tüm hizmet listesini doldur
                AllServices = await _context.Services.Where(s => !s.IsDeleted).ToListAsync()
            };

            return View(model);
        }

        // POST: Trainers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TrainerViewModel model)
        {
            if (id != model.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    // Güncellenecek veriyi DB'den çek (İlişkilerle beraber)
                    var trainerToUpdate = await _context.Trainers
                        .Include(t => t.TrainerServices)
                        .FirstOrDefaultAsync(t => t.Id == id);

                    if (trainerToUpdate == null) return NotFound();

                    // Temel alanları güncelle
                    trainerToUpdate.FullName = model.FullName;
                    trainerToUpdate.Bio = model.Bio;
                    trainerToUpdate.PhotoUrl = model.PhotoUrl;
                    trainerToUpdate.ShiftStart = model.ShiftStart;
                    trainerToUpdate.ShiftEnd = model.ShiftEnd;
                    trainerToUpdate.UpdatedDate = DateTime.Now;

                    // --- Çoka-Çok İlişki Güncelleme Mantığı ---

                    // 1. Mevcut ilişkileri temizle (En temiz yöntem budur)
                    // (Daha optimize yöntemler var ama öğrenci projesi için bu en güvenlisidir)
                    var existingServices = _context.TrainerServices.Where(ts => ts.TrainerId == id);
                    _context.TrainerServices.RemoveRange(existingServices);

                    // 2. Yeni seçilenleri ekle
                    if (model.SelectedServiceIds != null)
                    {
                        foreach (var serviceId in model.SelectedServiceIds)
                        {
                            _context.TrainerServices.Add(new TrainerService
                            {
                                TrainerId = id,
                                ServiceId = serviceId
                            });
                        }
                    }

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TrainerExists(model.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }

            model.AllServices = await _context.Services.Where(s => !s.IsDeleted).ToListAsync();
            return View(model);
        }

        // POST: Trainers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var trainer = await _context.Trainers.FindAsync(id);
            if (trainer != null)
            {
                // Soft Delete (Veriyi silme, IsDeleted işaretle)
                trainer.IsDeleted = true;
                trainer.UpdatedDate = DateTime.Now;

                // İstersen burada ilişkili randevuları da iptal edebilirsin
                // ama şimdilik sadece antrenörü pasife çekiyoruz.
                _context.Trainers.Update(trainer);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool TrainerExists(int id)
        {
            return _context.Trainers.Any(e => e.Id == id);
        }
    }
}