namespace SmartReadmeBuilder.Interfaces
{
    public interface IGitHubRepoConfig
    {
       Task<bool> AddFileToRepository(string username, string repo, string branch, string commitMessage, string githubToken, string markdown);
    }
}
