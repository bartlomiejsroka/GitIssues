using FluentAssertions;
using GitIssues.GitHub;
using GitIssues.Models;
using NSubstitute;

namespace GitIssues.Tests.GitHub
{
    public class GitHubIssueProviderTests
    {
        private readonly GitHubIssueProvider _provider;
        private readonly IGitHubHttpClient _gitHubHttpClient;
        private const string _token = "1234";
        private const string _userName = "test123";
        private const string _targetRepo = "repo";

        public GitHubIssueProviderTests()
        {
            _provider = new GitHubIssueProvider(_token, _userName, _targetRepo);
            _gitHubHttpClient = Substitute.For<IGitHubHttpClient>();
            _provider.ChangeClient(_gitHubHttpClient);
        }

        [Fact]
        public async Task GivenGitHubIssues_WhenExportIssues_ThenIssueCollectionReturned()
        {
            //given
            var issues = new List<Issue>()
            {
                new Issue("title 1", "desc 1"),
                new Issue("title 2", "desc 2")
            };
            var cancellationToken = new CancellationToken();
            _gitHubHttpClient.ExportAllIssues(_token, _userName, _targetRepo, cancellationToken).Returns(issues);

            //when
            var result = await _provider.ExportAll(cancellationToken);

            //then 
            result.Should().NotBeEmpty();
            result.Should().BeEquivalentTo(issues);
        }

        [Fact]
        public async Task GivenNoGitHubIssues_WhenExportIssues_ThenEmptyCollectionReturned()
        {
            //given
            var issues = Array.Empty<Issue>();
            var cancellationToken = new CancellationToken();
            _gitHubHttpClient.ExportAllIssues(_token, _userName, _targetRepo, cancellationToken).Returns(issues);

            //when
            var result = await _provider.ExportAll(cancellationToken);

            //then 
            result.Should().BeEmpty();
        }
    }
}