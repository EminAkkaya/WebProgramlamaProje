using System.Text;
using System.Text.Json;

namespace WebProgramlamaProje.Services
{
    public class GeminiService : IGeminiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public GeminiService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiKey = configuration["Gemini:ApiKey"];
        }

        public async Task<string> GetDietAndWorkoutPlanAsync(string promptText)
        {
            
            var url = "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent";

            // 2. Payload (Gidecek Veri)
            var payload = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new { text = promptText }
                        }
                    }
                }
            };

            var jsonContent = JsonSerializer.Serialize(payload);

            // 3. HTTP Request Oluşturma (Curl komutunun C# karşılığı)
            var request = new HttpRequestMessage(HttpMethod.Post, url);

            // ÖNEMLİ: Curl örneğindeki -H "x-goog-api-key: KEY" kısmı burası:
            request.Headers.Add("x-goog-api-key", _apiKey);

            // İçerik Tipi: application/json
            request.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            // 4. İsteği Gönder
            var response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();

                using var document = JsonDocument.Parse(responseString);
                try
                {
                    var resultText = document.RootElement
                        .GetProperty("candidates")[0]
                        .GetProperty("content")
                        .GetProperty("parts")[0]
                        .GetProperty("text")
                        .GetString();

                    return resultText ?? "Cevap boş döndü.";
                }
                catch
                {
                    return "API cevabı ayrıştırılamadı.";
                }
            }

            // Hata durumunda detay görmek için body'yi okuyalım
            var errorBody = await response.Content.ReadAsStringAsync();
            return $"Hata Kodu: {response.StatusCode} - Detay: {errorBody}";
        }
    }
}