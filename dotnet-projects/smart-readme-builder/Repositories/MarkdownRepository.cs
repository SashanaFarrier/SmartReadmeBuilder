using Microsoft.AspNetCore.Components.Forms;
using SmartReadmeBuilder.Models;
using System.Text.Json;
using System.Web;

namespace SmartReadmeBuilder.Repositories
{
    public class MarkdownRepository : IMarkdownRepository
    {
        private readonly IHttpContextAccessor _context;
        private readonly List<Prompt> _prompts;

        public MarkdownRepository(IHttpContextAccessor context)
        {
            _context = context;
            _prompts = GetPromptsFromSession();
        }

        private List<Prompt> GetPromptsFromSession()
        {
            var promptsJson = _context?.HttpContext?.Session.GetString("Prompts");
            return string.IsNullOrEmpty(promptsJson) ? new List<Prompt>() : JsonSerializer.Deserialize<List<Prompt>>(promptsJson);
        }

        public IEnumerable<Prompt> GetAllPrompts()
        {
            return _prompts;
        }

        public Prompt GetPromptById(Guid id)
        {
            return _prompts.FirstOrDefault(p => p.Id == id);
        }

        public void AddPrompt(Prompt prompt)
        {
            _prompts.Add(prompt);
        }

        public void UpdatePrompt(Prompt prompt)
        {
            var existingPrompt = _prompts?.FirstOrDefault(p => p.Id == prompt.Id);
            if (existingPrompt != null)
            {
                existingPrompt.Text = prompt.Text;
                //existingPrompt.MarkdownText = prompt.MarkdownText;
                //existingPrompt.CreatedOn = prompt.CreatedOn;
            }
        }

        public void DeletePrompt(Guid id)
        {
            var promptToRemove = _prompts?.FirstOrDefault(p => p.Id == id);
            if (promptToRemove != null)
            {
                _prompts?.Remove(promptToRemove);
            }
        }

        public void SaveChanges()
        {
           
            _context?.HttpContext?.Session.SetString("Prompts", JsonSerializer.Serialize(_prompts));
        }
    }
}
