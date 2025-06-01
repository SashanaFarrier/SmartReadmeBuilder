using Octokit;
using SmartReadmeBuilder.Models;
//using GitHub.Octokit.Client;
//using GitHub.Octokit.Client.Authentication;


namespace SmartReadmeBuilder.api
{
    public class GithubClient_API
    {
        //TokenProvider tokenProvider = new TokenProvider("your-github-token-here") ?? "";
        //RequestAdapter requestAdapter = RequestAdapter.Create(new TokenAuthProvider(tokenProvider));

        private readonly GitHubToken _githubToken;
        private string Owner { get; set; }
        private string Repo { get; set; }
        private string Branch { get; set; }
        private string FilePath { get; set; }
        private string commitMessage { get; set; }

        public GithubClient_API(GitHubToken githubToken)
        {
            _githubToken = githubToken;
        }

        public void ConfigureRepo(string owner, string repo, string branch, string commitMessage)
        {
            Owner = owner;
            Repo = repo;
            Branch = branch;
            //FilePath = filePath;
            this.commitMessage = commitMessage;

            
        }

        public async Task AddFileToRepository(string markdownText)
        {

            GitHubClient client = new GitHubClient(new ProductHeaderValue("SmartReadmeBuilder"));
            var tokenAuth = new Credentials(_githubToken.Token);
            client.Credentials = tokenAuth;

            var newBlob = new NewBlob
            {
                Content = markdownText,
                Encoding = EncodingType.Utf8
            };

            var createdBlob = await client.Git.Blob.Create(Owner, Repo, newBlob);
            //createdBlob.Dump();

            var newtree = new NewTree();
            newtree.Tree.Add(new NewTreeItem
            {
                Path = "README.md",
                Mode = Octokit.FileMode.File,
                Type = TreeType.Blob,
                Sha = createdBlob.Sha
            });

            var createdTree = await client.Git.Tree.Create(Owner, Repo, newtree);


            //var repoInfo = await client.Repository.Get(Owner, Repo);
            //var defaultBranch = repoInfo.DefaultBranch;
            //var master = await client.Git.Reference
            //.Get(Owner, Repo, "heads/master");
            var repo = await client.Repository.Get(Owner, Repo);
            var defaultBranch = repo.DefaultBranch;

            var branchReference = await client.Git.Reference.Get(Owner, Repo, "heads/" + defaultBranch);

           // var branchReference = await client.Git.Reference.Get(Owner, Repo, client.Repository.Get(Owner, Repo).Result.DefaultBranch);

            var newCommit = new NewCommit(
              "Added ReadMe",
              createdTree.Sha,
              new[] { branchReference.Object.Sha });


            var createdCommit = await client.Git.Commit
                .Create(Owner, Repo, newCommit);


            var updateReference = new ReferenceUpdate(createdCommit.Sha);
            var updatedReference = await client.Git
                .Reference.Update(Owner, Repo, "heads/" + defaultBranch, updateReference);

            var readme = await client.Repository.Content.GetAllContents(Owner, Repo, "README.md");


        }





    }


}
