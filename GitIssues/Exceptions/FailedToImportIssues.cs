using System.Net;

namespace GitIssues.Exceptions
{
    public class FailedToImportIssues : Exception
    {
        public FailedToImportIssues(HttpStatusCode statusCode, string message) : base(ExceptionMessageProvider.GetExceptionMessage(statusCode) + " " + message) { }
    }

}
