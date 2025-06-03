namespace SmartReadmeBuilder.Interfaces
{
    public interface IGitHubRepoConfig
    {
        //void ConfigureRepo(string owner, string repo, string branch, string commitMessage);
        Task AddFileToRepository(string owner, string repoName, string branch, string commitMessage, string githubToken, string markdownText);
    }
}
