using SmartReadmeBuilder.api;
using SmartReadmeBuilder.Models;
using SmartReadmeBuilder.Repositories;

namespace SmartReadmeBuilder.Services
{
    public class MarkdownService
    {
        private readonly IMarkdownRepository _markdownRepository;
        private readonly GithubClient_API _githubClient;

        public MarkdownService(IMarkdownRepository markdownRepository, GithubClient_API githubClient)
        {
            _markdownRepository = markdownRepository;
            _githubClient = githubClient;

        }
        public Markdown? GetMarkdown(Guid id)
        {
            return _markdownRepository.GetMarkdownById(id);
        }
        public void UpdateMarkdown(Markdown markdown)
        {
            var existing = _markdownRepository.GetMarkdownById(markdown.Id);
            if (existing != null)
            {
                existing.Text = markdown.Text;
                _markdownRepository.SaveChanges();
            }
        }

        public async Task<bool> PushMarkdownToRepo(Guid markdownId, string repo, string branch, string commitMessage)
        {
            var markdown = _markdownRepository.GetMarkdownById(markdownId);
            if (markdown == null) return false;

            return await _githubClient.AddFileToRepository(repo, branch, commitMessage, markdown.Text);
        }
    }
}
