using GitIssues.Exceptions;
using GitIssues.Extensions;
using GitIssues.Models;
using Newtonsoft.Json;
using System.Net;
using System.Text;

namespace GitIssues.GitLab
{
    internal interface IGitLabHttpClient
    {
        public Task<HttpStatusCode> CheckToken(string token, CancellationToken cancellationToken);

        public Task<HttpStatusCode> CreateIssue(string token, int projectId, Issue issue, CancellationToken cancellationToken);

        public Task<HttpStatusCode> UpdateIssue(string token, int projectId, int issueId, Issue issue, CancellationToken cancellationToken);

        public Task<HttpStatusCode> CloseIssue(string token, int projectId, int issueId, CancellationToken cancellationToken);

        public Task<IReadOnlyCollection<Issue>> ExportAllIssues(string token, int projectId, CancellationToken cancellationToken);

        public Task<HttpStatusCode> ImportIssues(string token, int projectId, IReadOnlyCollection<Issue> issues, CancellationToken cancellationToken);



    }
    internal class GitLabHttpClient : IGitLabHttpClient
    {
        private const string _baseUrl = "https://gitlab.com/api/v4";
        private const string _closeState = "close";

        public async Task<HttpStatusCode> CheckToken(string token, CancellationToken cancellationToken)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.AddGitLabHeadersToHttpClient(token);
                var url = $"{_baseUrl}/user";
                var response = await httpClient.GetAsync(url, cancellationToken);
                return response.StatusCode;
            }
        }

        public async Task<HttpStatusCode> CreateIssue(string token, int projectId, Issue issueRequest, CancellationToken cancellationToken)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.AddGitLabHeadersToHttpClient(token);

                var issueContent = new
                {
                    title = issueRequest.Title,
                    description = issueRequest.Description
                };

                var issueContentJson = JsonConvert.SerializeObject(issueContent);
                var content = new StringContent(issueContentJson, Encoding.UTF8, "application/json");
                var createIssueUrl = $"{_baseUrl}/projects/{projectId}/issues";

                var response = await httpClient.PostAsync(createIssueUrl, content, cancellationToken);
                return response.StatusCode;
            }
        }

        public async Task<HttpStatusCode> UpdateIssue(string token, int projectId, int issueId, Issue issueRequest, CancellationToken cancellationToken)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.AddGitLabHeadersToHttpClient(token);

                var issueContent = new
                {
                    title = issueRequest.Title,
                    description = issueRequest.Description
                };

                var issueContentJson = JsonConvert.SerializeObject(issueContent);
                var content = new StringContent(issueContentJson, Encoding.UTF8, "application/json");
                var updateIssueUrl = $"{_baseUrl}/projects/{projectId}/issues/{issueId}";

                var response = await httpClient.PutAsync(updateIssueUrl, content, cancellationToken);
                return response.StatusCode;
            }
        }

        public async Task<HttpStatusCode> CloseIssue(string token, int projectId, int issueId, CancellationToken cancellationToken)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.AddGitLabHeadersToHttpClient(token);

                var issueContent = new
                {
                    state_event = _closeState
                };

                var issueContentJson = JsonConvert.SerializeObject(issueContent);
                var content = new StringContent(issueContentJson, Encoding.UTF8, "application/json");
                var closeIssueUrl = $"{_baseUrl}/projects/{projectId}/issues/{issueId}";

                var response = await httpClient.PutAsync(closeIssueUrl, content, cancellationToken);
                return response.StatusCode;
            }

        }

        public async Task<IReadOnlyCollection<Issue>> ExportAllIssues(string token, int projectId, CancellationToken cancellationToken)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.AddGitLabHeadersToHttpClient(token);

                var resultIssues = new List<Issue>();
                var hasNextPage = true;
                var page = 1;
                var openStateFilter = "state=opened";

                while (hasNextPage)
                {
                    var getIssuesUrl = $"{_baseUrl}/projects/{projectId}/issues?page={page}&{openStateFilter}";
                    var response = await httpClient.GetAsync(getIssuesUrl, cancellationToken);
                    var responseContent = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        var issues = System.Text.Json.JsonSerializer.Deserialize<List<GitLabIssueDto>>(responseContent);
                        if(issues != null)
                            resultIssues.AddRange(issues.ToIssueModel());

                        hasNextPage = int.TryParse(response.Headers.GetValues("X-Next-Page").FirstOrDefault(), out var nextPage) && nextPage > page;
                        page++;
                    }
                    else
                        throw new FailedToRetreiveIssues(response.StatusCode);
                }

                return resultIssues;
            }
        }

        public async Task<HttpStatusCode> ImportIssues(string token, int projectId, IReadOnlyCollection<Issue> issues, CancellationToken cancellationToken)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.AddGitLabHeadersToHttpClient(token);
                var insertIssuesUrl = $"{_baseUrl}/projects/{projectId}/issues";

                foreach (var issue in issues)
                {
                    var issueToImport = new
                    {
                        title = issue.Title,
                        description = issue.Description
                    };

                    var issueJson = JsonConvert.SerializeObject(issueToImport);
                    var content = new StringContent(issueJson, Encoding.UTF8, "application/json");
                    var response = await httpClient.PostAsync(insertIssuesUrl, content, cancellationToken);

                    if (!response.IsSuccessStatusCode)
                        return response.StatusCode;
                }

                return HttpStatusCode.Created;
            }
        }
    }
}
