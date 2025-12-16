using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebProgramlamaProje.Data;
using WebProgramlamaProje.ViewModels;

namespace WebProgramlamaProje.Controllers
{
    [Route("api/[controller]")] // Erişim Adresi: /api/trainersapi
    [ApiController]
    public class TrainersApiController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TrainersApiController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/trainersapi
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TrainerDto>>> GetTrainers()
        {
            // Veritabanından veriyi çekiyoruz
            var trainers = await _context.Trainers
                .Where(t => !t.IsDeleted) // Silinenleri getirme
                .Include(t => t.TrainerServices)
                .ThenInclude(ts => ts.Service)
                .ToListAsync();

            // Entity nesnesini DTO'ya çeviriyoruz (Mapping)
            var trainerDtos = trainers.Select(t => new TrainerDto
            {
                Id = t.Id,
                FullName = t.FullName,
                Bio = t.Bio,
                PhotoUrl = string.IsNullOrEmpty(t.PhotoUrl)
                           ? "https://via.placeholder.com/300?text=Antrenor" // Foto yoksa varsayılan resim
                           : t.PhotoUrl,
                WorkingHours = $"{t.ShiftStart:hh\\:mm} - {t.ShiftEnd:hh\\:mm}",
                // İlişkili tablodan sadece hizmet isimlerini alıyoruz
                Specialties = t.TrainerServices.Select(ts => ts.Service.Name).ToList()
            }).ToList();

            return Ok(trainerDtos); // JSON olarak döner (HTTP 200)
        }
    }
}