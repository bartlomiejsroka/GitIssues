using GitIssues.Exceptions;
using GitIssues.Extensions;
using GitIssues.Models;
using Newtonsoft.Json;
using System.Net;
using System.Text;

namespace GitIssues.GitHub
{
    internal interface IGitHubHttpClient
    {
        public Task<HttpStatusCode> CheckToken(string token, CancellationToken cancellationToken);

        public Task<HttpStatusCode> CreateIssue(string token, string userName, string targetRepo, Issue issue, CancellationToken cancellationToken);

        public Task<HttpStatusCode> UpdateIssue(string token, string userName, string targetRepo, int IssueNumber, Issue issue, CancellationToken cancellationToken);

        public Task<HttpStatusCode> CloseIssue(string token, string userName, string targetRepo, int issueNumber, CancellationToken cancellationToken);

        public Task<IReadOnlyCollection<Issue>> ExportAllIssues(string token, string userName, string targetRepo, CancellationToken cancellationToken);

        public Task<HttpStatusCode> ImportIssues(string token, string userName, string targetRepo, IReadOnlyCollection<Issue> issues, CancellationToken cancellationToken);

    }

    internal class GitHubHttpClient : IGitHubHttpClient
    {
        private const string _baseRepoUrl = "https://api.github.com/repos";
        private const string _tokenCheckUrl = "https://api.github.com/octocat";
        private const string _closeState = "closed";

        public GitHubHttpClient()
        { }

        public async Task<HttpStatusCode> CheckToken(string token, CancellationToken cancellationToken)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.AddGitHubHeadersToHttpClient(token);
              
                var response = await httpClient.GetAsync(_tokenCheckUrl, cancellationToken);
                return response.StatusCode;
            }
        }

        public async Task<HttpStatusCode> CreateIssue(string token, string userName, string targetRepo, Issue issue, CancellationToken cancellationToken)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.AddGitHubHeadersToHttpClient(token);

                var issueContent = new
                {
                    title = issue.Title,
                    body = issue.Description
                };

                var issueContentJson = JsonConvert.SerializeObject(issueContent);
                var content = new StringContent(issueContentJson, Encoding.UTF8, "application/json");
                var createIssueUrl = $"{_baseRepoUrl}/{userName}/{targetRepo}/issues";

                var response = await httpClient.PostAsync(createIssueUrl, content, cancellationToken);
                return response.StatusCode;
            }
        }

        public async Task<HttpStatusCode> UpdateIssue(string token, string userName, string targetRepo, int issueNumber, Issue issue, CancellationToken cancellationToken)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.AddGitHubHeadersToHttpClient(token);

                var issueContent = new
                {
                    title = issue.Title,
                    body = issue.Description
                };

                var issueContentJson = JsonConvert.SerializeObject(issueContent);
                var content = new StringContent(issueContentJson, Encoding.UTF8, "application/json");
                var updateIssueUrl = $"{_baseRepoUrl}/{userName}/{targetRepo}/issues/{issueNumber}";

                var response = await httpClient.PatchAsync(updateIssueUrl, content, cancellationToken);
                return response.StatusCode;
            }
        }

        public async Task<HttpStatusCode> CloseIssue(string token, string userName, string targetRepo, int issueNumber, CancellationToken cancellationToken)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.AddGitHubHeadersToHttpClient(token);

                var issueContent = new
                {
                   state = _closeState
                };

                var issueContentJson = JsonConvert.SerializeObject(issueContent);
                var content = new StringContent(issueContentJson, Encoding.UTF8, "application/json");
                var closeIssueUrl = $"{_baseRepoUrl}/{userName}/{targetRepo}/issues/{issueNumber}";

                var response = await httpClient.PatchAsync(closeIssueUrl, content, cancellationToken);
                return response.StatusCode;
            }
        }

        public async Task<IReadOnlyCollection<Issue>> ExportAllIssues(string token, string userName, string targetRepo, CancellationToken cancellationToken)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.AddGitHubHeadersToHttpClient(token);
                var resultIssues = new List<Issue>();
                var hasNextPage = true;
                var page = 1;

                while (hasNextPage)
                {
                    var getIssuesUrl = $"{_baseRepoUrl}/{userName}/{targetRepo}/issues?page={page}";
                    var response = await httpClient.GetAsync(getIssuesUrl, cancellationToken);
                    var responseContent = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        var issues = System.Text.Json.JsonSerializer.Deserialize<List<GitHubIssueDto>>(responseContent);
                        
                        if(issues != null)
                            resultIssues.AddRange(issues.ToIssueModel());

                        hasNextPage = response.Headers.TryGetValues("Link", out var linkValues) && linkValues.Any(v => v.Contains("rel=\"next\""));
                        page++;
                    }

                    else
                       throw new FailedToRetreiveIssues(response.StatusCode);
                }

                return resultIssues;
            }
        }

        public async Task<HttpStatusCode> ImportIssues(string token, string userName, string targetRepo, IReadOnlyCollection<Issue> issues, CancellationToken cancellationToken)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.AddGitHubHeadersToHttpClient(token);
                var insertIssuesUrl = $"{_baseRepoUrl}/{userName}/{targetRepo}/issues";

                foreach (var issue in issues)
                {
                    var issueToImport = new
                    {
                        title = issue.Title,
                        body = issue.Description
                    };

                    var issueJson = JsonConvert.SerializeObject(issueToImport);
                    var content = new StringContent(issueJson, Encoding.UTF8, "application/json");
                    var response = await httpClient.PostAsync(insertIssuesUrl, content, cancellationToken);

                    if (!response.IsSuccessStatusCode)
                        throw new FailedToImportIssues(response.StatusCode, $"Error while importing issue : title: {issue.Title}, description: {issue.Description}");
                }

                return HttpStatusCode.Created;
            }
        }
    }
}
