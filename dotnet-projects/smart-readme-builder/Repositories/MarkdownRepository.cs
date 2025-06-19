using Microsoft.AspNetCore.Components.Forms;
using SmartReadmeBuilder.Models;
using System.Text.Json;
using System.Web;

namespace SmartReadmeBuilder.Repositories
{
    public class MarkdownRepository : IMarkdownRepository
    {
        private readonly IHttpContextAccessor _context;
        private readonly Log _log;

        public MarkdownRepository(IHttpContextAccessor context)
        {
            _context = context;
           _log = GetPromptsFromSession();
        }

        private Log GetPromptsFromSession()
        {
            var sessionHistoryLog = _context?.HttpContext?.Session.GetString("Logs");
            return string.IsNullOrEmpty(sessionHistoryLog) ? new Log() : JsonSerializer.Deserialize<Log>(sessionHistoryLog);
        }

        public Log GetLogs()
        {
            return _log; 
        }

        public IEnumerable<Prompt> GetAllPrompts()
        {
          return _log.Prompts;
             
        }

        public IEnumerable<Markdown> GetAllMarkdowns()
        {
            return _log.Markdowns;
        }


        public Prompt GetPromptById(Guid id)
        {
            return _log.Prompts.FirstOrDefault(p => p.Id == id);

        }

        public Markdown GetMarkdownById(Guid id)
        {
            return _log.Markdowns?.FirstOrDefault(m => m.Id == id);
        }

        public void AddPrompt(Prompt prompt)
        {
            _log.Prompts.Add(prompt);
        }

        public void AddMarkdown(Markdown markdown)
        {
            _log.Markdowns.Add(markdown);
        }

        public void UpdatePrompt(Prompt prompt)
        {
            var existingPrompt = _log.Prompts?.FirstOrDefault(p => p.Id == prompt.Id);
            if (existingPrompt != null)
            {
                existingPrompt.Text = prompt.Text;
            }
        }

        public void UpdateMarkdown(Markdown markdown)
        {
            var existingMarkdown = _log.Markdowns?.FirstOrDefault(p => p.Id == markdown.Id);

            if (existingMarkdown != null) 
            {
                existingMarkdown.Text = markdown.Text;
            }
        }

        public void DeletePrompt(Guid id)
        {
            var promptToRemove = _log.Prompts?.FirstOrDefault(p => p.Id == id);
            if (promptToRemove != null)
            {
                var markdownToRemove = _log.Markdowns?.FirstOrDefault(m => m.PromptId == id);
                _log.Prompts?.Remove(promptToRemove);
                _log.Markdowns?.Remove(markdownToRemove);
            }
        }

        public void DeleteMarkdown(Guid id)
        {
            var markdownToRemove = _log.Markdowns?.FirstOrDefault(m => m.Id == id);
            if (markdownToRemove != null)
            {
                _log.Markdowns?.Remove(markdownToRemove);
            }
        }

        public void SaveChanges()
        {
           _context.HttpContext.Session.SetString("Logs", JsonSerializer.Serialize(_log));
        }
    }
}
