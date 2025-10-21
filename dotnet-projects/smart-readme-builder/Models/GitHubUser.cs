using Jint.Runtime;
using Octokit;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SmartReadmeBuilder.Models
{
    public class GitHubUser
    {
        [JsonPropertyName("login")]
        public string? Login { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("email")]
        public string? Email { get; set; }

        [JsonPropertyName("avatar_url")]
        public string? AvatarUrl { get; set; }

        [JsonPropertyName("html_url")]
        public string? HtmlUrl { get; set; }
        
        public async Task<GitHubUser> GetGitHubUserInfo(string accessToken, HttpClient client)
        {
            //var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            client.DefaultRequestHeaders.UserAgent.ParseAdd("SmartReadMEBuilder");

            //Check if README already exists
            var response = await client.GetAsync("https://api.github.com/user");

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed to retrieve GitHub user info");
            }

            var json = await response.Content.ReadAsStringAsync();
            var user = JsonSerializer.Deserialize<GitHubUser>(json);

            return user;
        }

    }
}
