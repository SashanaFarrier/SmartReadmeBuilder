//using Octokit;
using SmartReadmeBuilder.api;
using SmartReadmeBuilder.Models;
using SmartReadmeBuilder.Repositories;
using System.Security.Claims;
using System.Text.RegularExpressions;


namespace SmartReadmeBuilder.Services
{
    public class LogService
    {
        private readonly IMarkdownRepository _markdownRepository;
        private readonly IHttpContextAccessor _context;

        public LogService(IMarkdownRepository markdownRepository, IHttpContextAccessor context)
        {
            _markdownRepository = markdownRepository;
            _context = context;
        }

        //public bool IsUserAuthenticated()
        //{
        //    //var user = _context.HttpContext?.User;
        //    //var username = user?.Identity?.Name; // GitHub login name
        //    //var claims = user?.Claims.ToList(); // All available claims

        //    //// You can also extract specific claims like:
        //    //var githubId = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        //    //var githubLogin = user?.FindFirst(ClaimTypes.Name)?.Value;
        //    return _context.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
        //}
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

                AIClient api = new AIClient();
                var response = await api.GetResponseAsync(prompt.Text);

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
                    AIClient api = new AIClient();

                    var response = await api.GetResponseAsync(existingPrompt.Text);

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
                AIClient api = new AIClient();
                
                var existingMarkdown = _markdownRepository.GetAllMarkdowns().ToList()
                        .Find(m => m.PromptId == existingPrompt.Id);
                
                var response = await api.GetResponseAsync(prompt.Text);
                    existingPrompt.Text = prompt.Text;
                    existingMarkdown.Text = response;

                    _markdownRepository.SaveChanges();

            } 
            catch (Exception ex)
            {
                ex.Message.ToString();
            }
                //check if closing punctuation in the form text and prompt text are the only diferences

                //if (Regex.Replace(existingPrompt.Text, @"[\p{P}]+$", "") == Regex.Replace(prompt.Text, @"[\p{P}]+$", ""))
                //{
                //    _markdownRepository.UpdatePrompt(prompt);
                //    _markdownRepository.SaveChanges();
                //}

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
