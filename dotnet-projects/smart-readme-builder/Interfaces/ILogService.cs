using SmartReadmeBuilder.Models;

namespace SmartReadmeBuilder.Interfaces
{
    public interface ILogService
    {
        public Log GetSessionLogHistory();
        Task<bool> CreateNewMarkdownText(Prompt prompt);
        Task<Prompt> RegeneratePromptResult(Guid id);
        Task<Prompt> GetExistingPrompt(Guid id);
        Task<Prompt> UpdateExistingPrompt(Prompt prompt);
        Task<bool> DeleteExistingPrompt(Guid id);
    }
}
