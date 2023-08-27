using System.Net;

namespace GitIssues.Exceptions
{
    public class FailedToRetreiveIssues : Exception
    {
        public FailedToRetreiveIssues(HttpStatusCode statusCode) : base(ExceptionMessageProvider.GetExceptionMessage(statusCode)) { }
    }

    internal static class ExceptionMessageProvider
    {
        public static string GetExceptionMessage(HttpStatusCode httpStatusCode)
        {
            return httpStatusCode switch
            {
                HttpStatusCode.NotFound => "Repo not found",
                HttpStatusCode.Unauthorized => "Bad token or no acees to repo",
                _ => "Unknown exception occurred"
            };
        }
    }
}
