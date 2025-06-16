using SmartReadmeBuilder.Models;

namespace SmartReadmeBuilder.Repositories
{
    public interface IMarkdownRepository
    {
        IEnumerable<Prompt> GetAllPrompts();
        Prompt GetPromptById(Guid id);
        void AddPrompt(Prompt prompt);
        void UpdatePrompt(Prompt prompt);
        void DeletePrompt(Guid id);
        void SaveChanges();
    }
}
