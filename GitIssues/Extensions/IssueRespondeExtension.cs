using GitIssues.GitHub;
using GitIssues.GitLab;
using GitIssues.Models;

namespace GitIssues.Extensions
{
    internal static class IssueRespondeExtension
    {
        public static IReadOnlyCollection<Issue> ToIssueModel(this List<GitHubIssueDto> gitHubIssueDtos)
            => gitHubIssueDtos.Select(x => new Issue(x.title, x.body)).ToList();

        public static IReadOnlyCollection<Issue> ToIssueModel(this List<GitLabIssueDto> gitHubIssueDtos)
           => gitHubIssueDtos.Select(x => new Issue(x.title, x.description)).ToList();
    }
}
