using Octokit;
using SmartReadmeBuilder.Interfaces;
using SmartReadmeBuilder.Models;


namespace SmartReadmeBuilder.api
{
    public class GithubClient_API : IGitHubRepoConfig
    {
        public async Task AddFileToRepository(string owner, string repoName, string branch, string commitMessage, string token, string markdownText)
        {

            GitHubClient client = new GitHubClient(new ProductHeaderValue("SmartReadmeBuilder"));
            //var token = Environment.GetEnvironmentVariable("GITHUB_TOKEN");

            if(!string.IsNullOrEmpty(token))
            {
                var tokenAuth = new Credentials(token);
                client.Credentials = tokenAuth;

                var newBlob = new NewBlob
                {
                    Content = markdownText,
                    Encoding = EncodingType.Utf8
                };

                var createdBlob = await client.Git.Blob.Create(owner, repoName, newBlob);

                var newtree = new NewTree();
                newtree.Tree.Add(new NewTreeItem
                {
                    Path = "README.md",
                    Mode = Octokit.FileMode.File,
                    Type = TreeType.Blob,
                    Sha = createdBlob.Sha
                });

                var createdTree = await client.Git.Tree.Create(owner, repoName, newtree);

                var repo = await client.Repository.Get(owner, repoName);
                //var defaultBranch = repo.DefaultBranch;

                var branchReference = await client.Git.Reference.Get(owner, repoName, "heads/" + branch);

                var newCommit = new NewCommit(
                  //"Added ReadMe",
                  commitMessage,
                  createdTree.Sha,
                  new[] { branchReference.Object.Sha });

                var createdCommit = await client.Git.Commit.Create(owner, repoName, newCommit);

                var updateReference = new ReferenceUpdate(createdCommit.Sha);

                //await client.Git.Reference.Update(owner, repoName, "heads/" + defaultBranch, updateReference);
                //await client.Repository.Content.GetAllContents(owner, repoName, "README.md");
                await client.Git.Reference.Update(owner, repoName, "heads/" + branch, updateReference);
                await client.Repository.Content.GetAllContents(owner, repoName, "README.md");

            }

        }

    }


}
