using FluentAssertions;
using GitIssues.GitLab;
using GitIssues.Models;
using NSubstitute;

namespace GitIssues.Tests.GitLab
{
    public class GitLabIssueProviderTests
    {
        private readonly GitLabIssueProvider _provider;
        private readonly IGitLabHttpClient _gitLabHttpClient;
        private const string _token = "1234";
        private const int _projectId = 22222;

        public GitLabIssueProviderTests()
        {
            _provider = new GitLabIssueProvider(_token, _projectId);
            _gitLabHttpClient = Substitute.For<IGitLabHttpClient>();
            _provider.ChangeClient(_gitLabHttpClient);
        }

        [Fact]
        public async Task GivenGitLabIssues_WhenExportIssues_ThenIssueCollectionReturned()
        {
            //given
            var issues = new List<Issue>()
            {
                new Issue("title 1", "desc 1"),
                new Issue("title 2", "desc 2")
            };
            var cancellationToken = new CancellationToken();
            _gitLabHttpClient.ExportAllIssues(_token, _projectId, cancellationToken).Returns(issues);

            //when
            var result = await _provider.ExportAll(cancellationToken);

            //then 
            result.Should().NotBeEmpty();
            result.Should().BeEquivalentTo(issues);
        }

        [Fact]
        public async Task GivenNoGitLabIssues_WhenExportIssues_ThenEmptyCollectionReturned()
        {
            //given
            var issues = Array.Empty<Issue>();
            var cancellationToken = new CancellationToken();
            _gitLabHttpClient.ExportAllIssues(_token, _projectId, cancellationToken).Returns(issues);

            //when
            var result = await _provider.ExportAll(cancellationToken);

            //then 
            result.Should().BeEmpty();
        }
    }
}
