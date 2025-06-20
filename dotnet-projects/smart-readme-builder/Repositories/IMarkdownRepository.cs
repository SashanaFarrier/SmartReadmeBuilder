using SmartReadmeBuilder.Models;

namespace SmartReadmeBuilder.Repositories
{
    public interface IMarkdownRepository
    {
        Log GetLogs();
        IEnumerable<Prompt> GetAllPrompts();
        IEnumerable<Markdown> GetAllMarkdowns();
        Prompt GetPromptById(Guid id);
        Markdown GetMarkdownById(Guid markdownId);
        void UpdateMarkdown(Markdown Markdown);
        void AddPrompt(Prompt prompt);
        void AddMarkdown(Markdown markdown);
        void UpdatePrompt(Prompt prompt);
        void DeletePrompt(Guid id);
        void DeleteMarkdown(Guid id);
        void SaveChanges();
    }
}
