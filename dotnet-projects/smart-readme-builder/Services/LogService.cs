//using Octokit;
using SmartReadmeBuilder.api;
using SmartReadmeBuilder.Interfaces;
using SmartReadmeBuilder.Models;
using SmartReadmeBuilder.Repositories;
using System.Security.Claims;
using System.Text.RegularExpressions;


namespace SmartReadmeBuilder.Services
{
    public class LogService : ILogService
    {
        private readonly AIClient _aiClient;
        private readonly IMarkdownRepository _markdownRepository;
        private readonly IHttpContextAccessor _context;

        public LogService(AIClient aiClient, IMarkdownRepository markdownRepository, IHttpContextAccessor context)
        {
            _aiClient = aiClient;
            _markdownRepository = markdownRepository;
            _context = context;
        }
        public Log GetSessionLogHistory()
        {
            var prompts = _markdownRepository.GetAllPrompts().OrderByDescending(p => p.CreatedOn).ToList();
            var markdowns = _markdownRepository.GetAllMarkdowns().OrderByDescending(m => m.CreatedOn).ToList();

            Log log = new Log
            {
                Prompts = prompts,
                Markdowns = markdowns
            };
            return log;
        }

        public async Task<bool> CreateNewMarkdownText(Prompt prompt)
        {
            try
            {
                var response = await _aiClient.GetResponseAsync(prompt.Text);

                Markdown markdown = new Markdown
                {
                    Text = response,
                    PromptId = prompt.Id
                };

                _markdownRepository.AddPrompt(prompt);
                _markdownRepository.AddMarkdown(markdown);
                _markdownRepository.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
                return false;
            }

        }

        public async Task<Prompt> RegeneratePromptResult(Guid id)
        {
            var existingPrompt = _markdownRepository.GetPromptById(id) ?? null;

            if (existingPrompt is not null)
            {
                var existingMarkdown = _markdownRepository.GetAllMarkdowns().ToList()
               .Find(m => m.PromptId == existingPrompt.Id);

                try
                {
                    var response = await _aiClient.GetResponseAsync(existingPrompt.Text);

                    existingMarkdown.Text = response;

                    _markdownRepository.SaveChanges();


                }
                catch (Exception ex)
                {
                    ex.Message.ToString();
                }

            }

            return existingPrompt;

        }

        public async Task<Prompt> GetExistingPrompt(Guid id)
        {
            var existingPrompt = _markdownRepository.GetPromptById(id);

            return existingPrompt;
        }

        public async Task<Prompt> UpdateExistingPrompt(Prompt prompt)
        {
            var existingPrompt = _markdownRepository.GetAllPrompts().FirstOrDefault(p => p.Id == prompt.Id) ?? null;

            if (existingPrompt?.Text == prompt.Text || string.IsNullOrEmpty(prompt.Text)) return existingPrompt;
                
            try
            {
                var existingMarkdown = _markdownRepository.GetAllMarkdowns().ToList()
                        .Find(m => m.PromptId == existingPrompt.Id);
                
                var response = await _aiClient.GetResponseAsync(prompt.Text);
                    existingPrompt.Text = prompt.Text;
                    existingMarkdown.Text = response;

                    _markdownRepository.SaveChanges();

            } 
            catch (Exception ex)
            {
                ex.Message.ToString();
            }
                
            return existingPrompt;

        }

        public async Task<bool> DeleteExistingPrompt(Guid id)
        {
            try
            {
                var existingPrompt = _markdownRepository.GetPromptById(id) ?? null;

                if (existingPrompt is not null)
                {
                    var existingMarkdown = _markdownRepository.GetAllMarkdowns().ToList()
                   .Find(m => m.PromptId == existingPrompt.Id);

                    _markdownRepository.DeletePrompt(id);
                    _markdownRepository.DeleteMarkdown(existingMarkdown.Id);
                    _markdownRepository.SaveChanges();
                   
                }

            }
            catch (Exception ex)
            {
                ex.Message.ToString();
                return false;
            }

            return true;
        }
            
    }   
}
