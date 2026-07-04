using SmartReadmeBuilder.Models;

namespace SmartReadmeBuilder.Interfaces
{
    public interface IMarkdownService
    {
        public Markdown GetMarkdown(Guid id);
        public void UpdateMarkdown(Markdown markdown);
        Task<bool> PushMarkdownToRepo(Guid markdownId, string repo, string branch, string commitMessage);
    }
}
