using Microsoft.AspNetCore.Mvc;
using WebProgramlamaProje.Services;
using WebProgramlamaProje.ViewModels;

namespace WebProgramlamaProje.Controllers
{
    public class AiController : Controller
    {
        private readonly IGeminiService _geminiService;

        public AiController(IGeminiService geminiService)
        {
            _geminiService = geminiService;
        }

        // GET: Sayfayı Göster
        public IActionResult Index()
        {
            return View(new AiPlanViewModel());
        }

        // POST: Form gönderilince çalışır
        [HttpPost]
        public async Task<IActionResult> Index(AiPlanViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Prompt Mühendisliği (Prompt Engineering) Kısmı
            // Yapay zekaya HTML formatında cevap vermesini söylüyoruz, böylece ekranda şık durur.
            string prompt = $@"
                Sen uzman bir spor hocası ve diyetisyensin. Aşağıdaki özelliklere sahip bir kişi için;
                1. Günlük örnek bir beslenme programı (Kahvaltı, Öğle, Akşam, Ara öğün),
                2. Haftalık kısa bir egzersiz rutini (Hangi günler ne yapmalı) hazırla.
                
                Kişi Bilgileri:
                - Cinsiyet: {model.Gender}
                - Yaş: {model.Age}
                - Boy: {model.Height} cm
                - Kilo: {model.Weight} kg
                - Aktivite Seviyesi: {model.ActivityLevel}
                - Hedef: {model.Goal}

                Lütfen cevabı HTML formatında ver (div, h3, ul, li, p etiketlerini kullan). 
                Sadece içeriği ver, ```html gibi markdown işaretleri kullanma.
                Başlıkları belirgin yap. Samimi ve motive edici bir dil kullan.";

            // Servise gönder ve cevabı bekle
            string aiResponse = await _geminiService.GetDietAndWorkoutPlanAsync(prompt);

            // Cevabı Modele ekleyip View'a geri gönder
            model.AiResponse = aiResponse;

            return View(model);
        }
    }
}