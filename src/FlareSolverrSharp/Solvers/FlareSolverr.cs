using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FlareSolverrSharp.Exceptions;
using FlareSolverrSharp.Types;
using FlareSolverrSharp.Utilities;
using Newtonsoft.Json;

namespace FlareSolverrSharp.Solvers
{
    public class FlareSolverr
    {
        private static readonly SemaphoreLocker Locker = new SemaphoreLocker();
        private HttpClient _httpClient;
        private readonly Uri _flareSolverrUri;

        public int MaxTimeout = 60000;

        public FlareSolverr(string flareSolverrApiUrl)
        {
            var apiUrl = flareSolverrApiUrl;
            if (!apiUrl.EndsWith("/"))
                apiUrl += "/";
            _flareSolverrUri = new Uri(apiUrl + "v1");
        }

        public async Task<FlareSolverrResponse> Solve(HttpRequestMessage request)
        {
            FlareSolverrResponse result = null;

            await Locker.LockAsync(async () =>
            {
                HttpResponseMessage response;
                try
                {
                    _httpClient = new HttpClient();
                    response = await _httpClient.PostAsync(_flareSolverrUri, GenerateFlareSolverrRequest(request));
                }
                catch (HttpRequestException e)
                {
                    throw new FlareSolverrException("Error connecting to FlareSolverr server: " + e);
                }
                catch (Exception e)
                {
                    throw new FlareSolverrException("Exception: " + e.ToString());
                }
                finally
                {
                    _httpClient.Dispose();
                }

                // Don't try parsing if FlareSolverr hasn't returned 200 or 500
                if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.InternalServerError)
                {
                    throw new FlareSolverrException("HTTP StatusCode not 200 or 500. Status is :" + response.StatusCode);
                }

                var resContent = await response.Content.ReadAsStringAsync();
                try
                {
                    result = JsonConvert.DeserializeObject<FlareSolverrResponse>(resContent);
                }
                catch (Exception)
                {
                    throw new FlareSolverrException("Error parsing response, check FlareSolverr. Response: " + resContent);
                }

                    try
                {
                    Enum.TryParse(result.Status, true, out FlareSolverrStatusCode returnStatusCode);

                    if (returnStatusCode.Equals(FlareSolverrStatusCode.ok))
                    {
                        return result;
                    }
                    else if (returnStatusCode.Equals(FlareSolverrStatusCode.warning))
                    {
                        throw new FlareSolverrException(
                            "FlareSolverr was able to process the request, but a captcha was detected. Message: "
                            + result.Message);
                    }
                    else if (returnStatusCode.Equals(FlareSolverrStatusCode.error))
                    {
                        throw new FlareSolverrException(
                            "FlareSolverr was unable to process the request, please check FlareSolverr logs. Message: "
                            + result.Message);
                    }
                    else
                    {
                        throw new FlareSolverrException("Unable to map FlareSolverr returned status code, received code: "
                            + result.Status + ". Message: " + result.Message);
                    }
                }
                catch (ArgumentException)
                {
                    throw new FlareSolverrException("Error parsing status code, check FlareSolverr log. Status: "
                            + result.Status + ". Message: " + result.Message);
                }
            });

            return result;
        }

        private HttpContent GenerateFlareSolverrRequest(HttpRequestMessage request)
        {
            FlareSolverrRequest req;

            if (request.Method == HttpMethod.Get)
            {
                req = new FlareSolverrRequestGet
                {
                    Cmd = "request.get",
                    Url = request.RequestUri.ToString(),
                    MaxTimeout = MaxTimeout
                };
            }
            else if (request.Method == HttpMethod.Post)
            {
                throw new FlareSolverrException("Not currently implemented HttpMethod: POST");
            }
            else
            {
                throw new FlareSolverrException("Unsupported HttpMethod: " + request.Method.ToString());
            }

            var userAgent = request.Headers.UserAgent.ToString();
            if (!string.IsNullOrWhiteSpace(userAgent))
                req.UserAgent = userAgent;

            var payload = JsonConvert.SerializeObject(req);
            HttpContent content = new StringContent(payload, Encoding.UTF8, "application/json");
            return content;
        }
 
    }
}