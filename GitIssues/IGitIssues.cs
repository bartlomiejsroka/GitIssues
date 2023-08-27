using GitIssues.Models;
using System.Net;
using GitIssues.Exceptions;

namespace GitIssues
{
    public interface IGitIssues
    {

        /// <summary>
        /// Create an issue
        /// </summary>
        /// <param name="issue">Object containg issue details</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Status code 201 if succeed</returns>
        public Task<HttpStatusCode> CreateIssue(Issue issue, CancellationToken cancellationToken);

        /// <summary>
        /// Update existing issue
        /// </summary>
        /// <param name="issueNumber">Issue number</param>
        /// <param name="issue">Object containg issue details</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Status code 201 if succeed</returns>
        public Task<HttpStatusCode> UpdateIssue(int issueNumber, Issue issue, CancellationToken cancellationToken);

        /// <summary>
        /// Close Issue
        /// </summary>
        /// <param name="issueNumber">Issue number</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Status code 200 if succeed</returns>
        public Task<HttpStatusCode> CloseIssue(int issueNumber, CancellationToken cancellationToken);

        /// <summary>
        /// Get all open issues from repository
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Collection of issues</returns>
        /// <exception cref="FailedToRetreiveIssues">When server error occurs </exception>
        public Task<IReadOnlyCollection<Issue>> ExportAll(CancellationToken cancellationToken);

        /// <summary>
        /// Import all issues to git repository
        /// </summary>
        /// <param name="issueRequests">Collection of issues</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Status code 201 if succeed</returns>
        /// <exception cref="FailedToImportIssues">When server error occurs</exception>
        public Task<HttpStatusCode> Import(IReadOnlyCollection<Issue> issueRequests, CancellationToken cancellationToken); //todo

        /// <summary>
        /// Check if given token is valid
        /// </summary>
        /// <param name="token">Git personal token</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Status code 200 if succeed</returns>
        public Task<HttpStatusCode> CheckToken(string token, CancellationToken cancellationToken);

        /// <summary>
        /// Chenge user token
        /// </summary>
        /// <param name="token">Git personal token</param>
        public void ChangeToken(string token);
    }
}