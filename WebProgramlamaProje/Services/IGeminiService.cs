namespace WebProgramlamaProje.Services
{
    public interface IGeminiService
    {
        Task<string> GetDietAndWorkoutPlanAsync(string prompt);
    }
}