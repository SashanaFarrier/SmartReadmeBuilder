using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.HttpResults;
//using Octokit;
using SmartReadmeBuilder.Interfaces;
using SmartReadmeBuilder.Models;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;


namespace SmartReadmeBuilder.api
{
    public class GithubClient_API : IGitHubRepoConfig
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public GithubClient_API(IHttpContextAccessor context)
        {
            _httpContextAccessor = context;
        }

        public async Task<bool> AddFileToRepository(string repo, string branch, string commitMessage, string markdownText)
        {

            var client = new HttpClient();
            var httpContext = _httpContextAccessor.HttpContext;
            
            if (httpContext == null)
            {
                throw new InvalidOperationException("HttpContext is not available.");
            }

            var accessToken = await httpContext.GetTokenAsync("access_token");
            var userInfo = await new GitHubUser().GetGitHubUserInfo(accessToken, client);

            var getUrl = $"https://api.github.com/repos/{userInfo.Login}/{repo}/contents/README.md?ref={branch}";
            var getResponse = await client.GetAsync(getUrl);

            string sha = null;
            if (getResponse.IsSuccessStatusCode)
            {
                var existingFile = JsonDocument.Parse(await getResponse.Content.ReadAsStringAsync());
                sha = existingFile.RootElement.GetProperty("sha").GetString();
            }

            // Step 2: Prepare the request body
            var payload = new
            {
                message = commitMessage,
                content = Convert.ToBase64String(Encoding.UTF8.GetBytes(markdownText)),
                branch = branch,
                sha = sha // include only if updating
            };

            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Step 3: PUT the file
            var putUrl = $"https://api.github.com/repos/{userInfo.Login}/{repo}/contents/README.md";
            var putResponse = await client.PutAsync(putUrl, content);

            if (putResponse.IsSuccessStatusCode)
            {
                //return Ok("README.md pushed successfully!");
                return true;
            }
            else
            {
                var error = await putResponse.Content.ReadAsStringAsync();
                //return StatusCode((int)putResponse.StatusCode, $"GitHub API error: {error}");

                return false;
            }

        }
    }
}
