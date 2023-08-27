using System.Net.Http.Headers;

namespace GitIssues.Extensions
{
    internal static class HttpClientExtension
    {
        public static void AddGitHubHeadersToHttpClient(this HttpClient httpClient, string token)
        {
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("token", token);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));
            httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("bartlomiejSroka");
        }

        public static void AddGitLabHeadersToHttpClient(this HttpClient httpClient, string token)
        {
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Add("Private-Token", token);
        }
    }
}
