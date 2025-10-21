using Octokit;

namespace SmartReadmeBuilder.Interfaces
{
    public interface IGitHubRepoConfig
    {
       Task<bool> AddFileToRepository(string repo, string branch, string commitMessage, string markdown);
    }
}
