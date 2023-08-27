using GitIssues.Models;
using System.Net;

namespace GitIssues.GitLab
{
    public class GitLabIssueProvider : IGitIssues
    {
        private string _token;
        private IGitLabHttpClient _httpClient;
        private readonly int _projectId;

        public GitLabIssueProvider(string token, int projectId)
        {
            _token = token;
            _projectId = projectId;
            _httpClient = new GitLabHttpClient();
        }

        public void ChangeToken(string token)
        {
            _token = token;
        }

        public Task<HttpStatusCode> CheckToken(string token, CancellationToken cancellationToken)
        {
           return _httpClient.CheckToken(token, cancellationToken);
        }

        public Task<HttpStatusCode> CloseIssue(int issueNumber, CancellationToken cancellationToken)
        {
            return _httpClient.CloseIssue(_token, _projectId, issueNumber, cancellationToken);
        }

        public Task<HttpStatusCode> CreateIssue(Issue issueRequest, CancellationToken cancellationToken)
        {
            return _httpClient.CreateIssue(_token, _projectId, issueRequest, cancellationToken);
        }

        public Task<IReadOnlyCollection<Issue>> ExportAll(CancellationToken cancellationToken)
        {
            return _httpClient.ExportAllIssues(_token, _projectId, cancellationToken);
        }

        public Task<HttpStatusCode> UpdateIssue(int issueNumber, Issue issue, CancellationToken cancellationToken)
        {
            return _httpClient.UpdateIssue(_token, _projectId, issueNumber, issue, cancellationToken);
        }

        public Task<HttpStatusCode> Import(IReadOnlyCollection<Issue> issues, CancellationToken cancellationToken)
        {
            return _httpClient.ImportIssues(_token, _projectId, issues, cancellationToken);
        }

        internal void ChangeClient(IGitLabHttpClient client)
        {
            _httpClient = client;
        }
    }
}
