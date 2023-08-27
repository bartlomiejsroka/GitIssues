using GitIssues.Models;
using System.Net;

namespace GitIssues.GitHub
{
    public class GitHubIssueProvider : IGitIssues
    {
        private string _token;
        private IGitHubHttpClient _httpClient;
        private readonly string _userRepo;
        private readonly string _userName;

        public GitHubIssueProvider(string token, string userName, string targetRepo)
        {
            _token = token;
            _userName = userName;
            _userRepo = targetRepo;
            _httpClient = new GitHubHttpClient();
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
            return _httpClient.CloseIssue(_token, _userName, _userRepo, issueNumber, cancellationToken);
        }

        public Task<HttpStatusCode> CreateIssue(Issue issueRequest, CancellationToken cancellationToken)
        {
            return _httpClient.CreateIssue(_token, _userName, _userRepo, issueRequest, cancellationToken);
        }

        public Task<HttpStatusCode> UpdateIssue(int issueNumber, Issue issueRequest, CancellationToken cancellationToken)
        {
            return _httpClient.UpdateIssue(_token, _userName, _userRepo, issueNumber, issueRequest, cancellationToken);
        }

        public Task<IReadOnlyCollection<Issue>> ExportAll(CancellationToken cancellationToken)
        {
            return _httpClient.ExportAllIssues(_token, _userName, _userRepo, cancellationToken);
        }

        public Task<HttpStatusCode> Import(IReadOnlyCollection<Issue> issueRequests, CancellationToken cancellationToken)
        {
            return _httpClient.ImportIssues(_token, _userName, _userRepo, issueRequests, cancellationToken);
        }

        internal void ChangeClient(IGitHubHttpClient client)
        {
            _httpClient = client;
        }
    }
}
