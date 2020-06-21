using System.Net.Http;

namespace FlareSolverrSharp.Exceptions
{
    /// <summary>
    /// The exception that is thrown if FlareSolverr fails
    /// </summary>
    public class FlareSolverrException : HttpRequestException
    {
        public FlareSolverrException(string message) : base(message)
        {
        }
    }
}
