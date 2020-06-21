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
                    throw new FlareSolverrException(e.ToString());
                }
                finally
                {
                    _httpClient.Dispose();
                }

                var resContent = await response.Content.ReadAsStringAsync();
                try
                {
                    result = JsonConvert.DeserializeObject<FlareSolverrResponse>(resContent);
                }
                catch (Exception)
                {
                    throw new FlareSolverrException("Error parsing response, check FlareSolverr version. Response: " + resContent);
                }

                if (response.StatusCode != HttpStatusCode.OK)
                    throw new FlareSolverrException(result.Message);
            });

            return result;
        }

        private HttpContent GenerateFlareSolverrRequest(HttpRequestMessage request)
        {
            var req = new FlareSolverrRequest
            {
                Method = "GET",
                Url = request.RequestUri.ToString(),
                MaxTimeout = MaxTimeout
            };

            var userAgent = request.Headers.UserAgent.ToString();
            if (!string.IsNullOrWhiteSpace(userAgent))
                req.UserAgent = userAgent;

            var payload = JsonConvert.SerializeObject(req);
            HttpContent content = new StringContent(payload, Encoding.UTF8, "application/json");
            return content;
        }
 
    }
}