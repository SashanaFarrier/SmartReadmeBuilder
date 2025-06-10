namespace SmartReadmeBuilder.Interfaces
{
    public interface IGitHubRepoConfig
    {
        //void ConfigureRepo(string owner, string repo, string branch, string commitMessage);
       Task<bool> AddFileToRepository(string username, string repo, string branch, string commitMessage, string githubToken, string markdown);
    }
}
