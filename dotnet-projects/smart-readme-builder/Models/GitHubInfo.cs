using System.ComponentModel.DataAnnotations;

namespace SmartReadmeBuilder.Models
{
    public class GitHubInfo
    {
        public Guid MarkdownId { get; set; }
        [Required]
        public string? Username { get; set; }
        [Required]
        public string? Repo { get; set; }
        [Required]
        public string? Branch { get; set; }
        [Required]
        public string? CommitMessage { get; set; }
        [Required]
        public string? GithubToken { get; set; }
    }
}
