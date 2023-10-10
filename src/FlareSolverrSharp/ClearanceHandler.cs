using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FlareSolverrSharp.Constants;
using FlareSolverrSharp.Exceptions;
using FlareSolverrSharp.Extensions;
using FlareSolverrSharp.Solvers;
using FlareSolverrSharp.Types;
using Cookie = System.Net.Cookie;

namespace FlareSolverrSharp
{
    /// <summary>
    /// A HTTP handler that transparently manages CloudFlare's protection bypass.
    /// </summary>
    public class ClearanceHandler : DelegatingHandler
    {
        private readonly HttpClient _client;
        private readonly string _flareSolverrApiUrl;
        private FlareSolverr _flareSolverr;
        private string _userAgent;

        /// <summary>
        /// Max timeout to solve the challenge.
        /// </summary>
        public int MaxTimeout = 60000;

        /// <summary>
        /// HTTP Proxy URL.
        /// Example: http://127.0.0.1:8888
        /// </summary>
        public string ProxyUrl = "";

        /// <summary>
        /// HTTP Proxy Username.
        /// </summary>
        public string ProxyUsername = null;

        /// <summary>
        /// HTTP Proxy Password.
        /// </summary>
        public string ProxyPassword = null;

        private HttpClientHandler HttpClientHandler => InnerHandler.GetMostInnerHandler() as HttpClientHandler;

        /// <summary>
        /// Creates a new instance of the <see cref="ClearanceHandler"/>.
        /// </summary>
        /// <param name="flareSolverrApiUrl">FlareSolverr API URL. If null or empty it will detect the challenges, but
        /// they will not be solved. Example: "http://localhost:8191/"</param>
        public ClearanceHandler(string flareSolverrApiUrl)
            : base(new HttpClientHandler())
        {
            // Validate URI
            if (!string.IsNullOrWhiteSpace(flareSolverrApiUrl)
                && !Uri.IsWellFormedUriString(flareSolverrApiUrl, UriKind.Absolute))
                throw new FlareSolverrException("FlareSolverr URL is malformed: " + flareSolverrApiUrl);

            _flareSolverrApiUrl = flareSolverrApiUrl;

            _client = new HttpClient(new HttpClientHandler
            {
                AllowAutoRedirect = false,
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
                CookieContainer = new CookieContainer()
            });
        }

        /// <summary>
        /// Sends an HTTP request to the inner handler to send to the server as an asynchronous operation.
        /// </summary>
        /// <param name="request">The HTTP request message to send to the server.</param>
        /// <param name="cancellationToken">A cancellation token to cancel operation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // Init FlareSolverr
            if (_flareSolverr == null && !string.IsNullOrWhiteSpace(_flareSolverrApiUrl))
            {
                _flareSolverr = new FlareSolverr(_flareSolverrApiUrl)
                {
                    MaxTimeout = MaxTimeout,
                    ProxyUrl = ProxyUrl,
                    ProxyUsername = ProxyUsername,
                    ProxyPassword = ProxyPassword
                };
            }

            // Set the User-Agent if required
            SetUserAgentHeader(request);

            // Perform the original user request
            var response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);

            // Detect if there is a challenge in the response
            if (ChallengeDetector.IsClearanceRequired(response))
            {
                if (_flareSolverr == null)
                    throw new FlareSolverrException("Challenge detected but FlareSolverr is not configured");

                // Resolve the challenge using FlareSolverr API
                var flareSolverrResponse = await _flareSolverr.Solve(request);

                // Save the FlareSolverr User-Agent for the following requests
                var flareSolverUserAgent = flareSolverrResponse.Solution.UserAgent;
                if (flareSolverUserAgent != null && !flareSolverUserAgent.Equals(request.Headers.UserAgent.ToString()))
                {
                    _userAgent = flareSolverUserAgent;

                    // Set the User-Agent if required
                    SetUserAgentHeader(request);
                }

                // Change the cookies in the original request with the cookies provided by FlareSolverr
                InjectCookies(request, flareSolverrResponse);
                response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);

                // Detect if there is a challenge in the response
                if (ChallengeDetector.IsClearanceRequired(response))
                    throw new FlareSolverrException("The cookies provided by FlareSolverr are not valid");

                // Add the "Set-Cookie" header in the response with the cookies provided by FlareSolverr
                InjectSetCookieHeader(response, flareSolverrResponse);
            }

            return response;
        }

        private void SetUserAgentHeader(HttpRequestMessage request)
        {
            if (_userAgent != null)
            {
                // Overwrite the header
                request.Headers.Remove(HttpHeaders.UserAgent);
                request.Headers.Add(HttpHeaders.UserAgent, _userAgent);
            }
        }

        private void InjectCookies(HttpRequestMessage request, FlareSolverrResponse flareSolverrResponse)
        {
            var flareCookies = flareSolverrResponse.Solution.Cookies.ToList();

            // use only Cloudflare and DDoS-GUARD cookies
            flareCookies = flareCookies.Where(cookie =>
                cookie.Name.StartsWith("cf_")
                || cookie.Name.StartsWith("__cf")
                || cookie.Name.StartsWith("__ddg")).ToList();

            if (!flareCookies.Any())
                return;

            if (HttpClientHandler.UseCookies)
            {
                var currentCookies = HttpClientHandler.CookieContainer.GetCookies(request.RequestUri);

                // remove previous FlareSolverr cookies
                var expiredCount = 0;
                foreach (var flareCookie in flareCookies)
                {
                    var cookie = currentCookies[flareCookie.Name];
                    if (cookie == null)
                        continue;
                    cookie.Expired = true;
                    expiredCount += 1;
                }

                // there is a max number of cookies, we have to make space (we assume the first
                var cookieExcess = currentCookies.Count + flareCookies.Count
                                   - expiredCount - HttpClientHandler.CookieContainer.PerDomainCapacity;
                foreach (Cookie cookie in currentCookies)
                {
                    if (cookieExcess == 0)
                        break;
                    if (cookie.Expired)
                        continue;
                    cookie.Expired = true;
                    cookieExcess -= 1;
                }

                // add FlareSolverr cookies
                foreach (var rCookie in flareCookies)
                    HttpClientHandler.CookieContainer.Add(request.RequestUri, rCookie.ToCookieObj());
            }
            else
            {
                foreach (var rCookie in flareCookies)
                    request.Headers.Add(HttpHeaders.Cookie, rCookie.ToHeaderValue());
            }
        }

        private void InjectSetCookieHeader(HttpResponseMessage response, FlareSolverrResponse flareSolverrResponse)
        {
            var rCookies = flareSolverrResponse.Solution.Cookies;
            if (!rCookies.Any())
                return;

            // inject set-cookie headers in the response
            foreach (var rCookie in rCookies)
                response.Headers.Add(HttpHeaders.SetCookie, rCookie.ToHeaderValue());
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                _client.Dispose();

            base.Dispose(disposing);
        }

    }
}
