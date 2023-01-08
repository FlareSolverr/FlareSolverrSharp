using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using FlareSolverrSharp.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FlareSolverrSharp.Tests
{
    [TestClass]
    public class ClearanceHandlerTests
    {

        [TestMethod]
        public async Task SolveOk()
        {
            var uri = new Uri("https://www.google.com/");
            var handler = new ClearanceHandler(Settings.FlareSolverrApiUrl)
            {
                MaxTimeout = 60000
            };

            var client = new HttpClient(handler);
            var response = await client.GetAsync(uri);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [TestMethod]
        public async Task SolveOkCloudflareGet()
        {
            var handler = new ClearanceHandler(Settings.FlareSolverrApiUrl)
            {
                MaxTimeout = 60000
            };

            var client = new HttpClient(handler);
            var response = await client.GetAsync(Settings.ProtectedUri);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [TestMethod]
        public async Task SolveOkCloudflareGetManyCookies()
        {
            // there is a limit in the maximum number of cookies that CookieContainer could have
            // we implemented some logic to add Cloudflare cookies even if the container is full
            // prepare a container full of cookies
            var url = Settings.ProtectedUri;
            var cookiesContainer = new CookieContainer
            {
                PerDomainCapacity = 5 // by default is 25
            };
            var cookieUrl = new Uri(url.Scheme + "://" + url.Host);
            for (var i = 0; i < 6; i++)
                cookiesContainer.Add(cookieUrl, new Cookie($"cookie{i}", $"value{i}"));
            var cookies = cookiesContainer.GetCookies(url);
            Assert.AreEqual(5, cookies.Count); // the first cookie0 is lost
            Assert.AreEqual("cookie1", cookies.First().Name);

            // prepare the client
            var clientHandler = new HttpClientHandler
            {
                CookieContainer = cookiesContainer,
                AllowAutoRedirect = false, // Do not use this - Bugs ahoy! Lost cookies and more.
                UseCookies = true,
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };
            var handler = new ClearanceHandler(Settings.FlareSolverrApiUrl)
            {
                MaxTimeout = 60000
            };
            handler.InnerHandler = clientHandler;

            var client = new HttpClient(handler);
            var response = await client.GetAsync(url);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            // we check the cookies again
            cookies = cookiesContainer.GetCookies(url);
            Assert.AreEqual(5, cookies.Count); 
            Assert.IsNotNull(cookies["cf_clearance"]);
        }

        [TestMethod]
        public async Task SolveOkCloudflarePost()
        {
            var handler = new ClearanceHandler(Settings.FlareSolverrApiUrl)
            {
                MaxTimeout = 60000
            };

            var request = new HttpRequestMessage();
            request.Headers.ExpectContinue = false;
            request.RequestUri = Settings.ProtectedPostUri;
            var postData = new Dictionary<string, string> { { "story", "test" }};
            request.Content = FormUrlEncodedContentWithEncoding(postData, Encoding.UTF8);
            request.Method = HttpMethod.Post;

            var client = new HttpClient(handler);
            var response = await client.SendAsync(request);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [TestMethod]
        public async Task SolveOkCloudflareUserAgentHeader()
        {
            var handler = new ClearanceHandler(Settings.FlareSolverrApiUrl)
            {
                MaxTimeout = 60000
            };

            var request = new HttpRequestMessage(HttpMethod.Get, Settings.ProtectedUri);
            request.Headers.UserAgent.ParseAdd("Mozilla/5.0 (X11; Linux x86_64; rv:108.0) Gecko/20100101 Firefox/108.0");

            var client = new HttpClient(handler);
            var response = await client.SendAsync(request);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            // The request User-Agent will be replaced with FlareSolverr User-Agent
            Assert.IsTrue(request.Headers.UserAgent.ToString().Contains("Chrome/"));
        }

        [TestMethod]
        public async Task SolveOkProxy()
        {
            var handler = new ClearanceHandler(Settings.FlareSolverrApiUrl)
            {
                MaxTimeout = 60000,
                ProxyUrl = Settings.ProxyUrl
            };

            var client = new HttpClient(handler);
            var response = await client.GetAsync(Settings.ProtectedUri);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [TestMethod]
        public async Task SolveOkCloudflareDDoSGuardGet()
        {
            var handler = new ClearanceHandler(Settings.FlareSolverrApiUrl)
            {
                MaxTimeout = 60000
            };

            var client = new HttpClient(handler);
            var response = await client.GetAsync(Settings.ProtectedDdgUri);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.IsTrue(!response.Content.ReadAsStringAsync().Result.ToLower().Contains("ddos"));
        }

        [TestMethod]
        public async Task SolveOkCloudflareCustomGet()
        {
            // Custom CloudFlare for EbookParadijs, Film-Paleis, MuziekFabriek and Puur-Hollands
            var handler = new ClearanceHandler(Settings.FlareSolverrApiUrl)
            {
                MaxTimeout = 60000
            };

            var client = new HttpClient(handler);
            var response = await client.GetAsync(Settings.ProtectedCcfUri);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.IsTrue(!response.Content.ReadAsStringAsync().Result.ToLower().Contains("ddos"));
        }

        [TestMethod]
        public async Task SolveErrorCloudflareBlockedGet()
        {
            var handler = new ClearanceHandler(Settings.FlareSolverrApiUrl)
            {
                MaxTimeout = 60000
            };

            var client = new HttpClient(handler);
            try
            {
                await client.GetAsync(Settings.ProtectedBlockedUri);
                Assert.Fail("Exception not thrown");
            }
            catch (FlareSolverrException e)
            {
                Assert.IsTrue(e.Message.Contains("Error solving the challenge. Cloudflare has blocked this request. Probably your IP is banned for this site"));
            }
            catch (Exception e)
            {
                Assert.Fail("Unexpected exception: " + e);
            }
        }

        [TestMethod]
        public async Task SolveErrorUrl()
        {
            var uri = new Uri("https://www.google.bad1/");
            var handler = new ClearanceHandler(Settings.FlareSolverrApiUrl)
            {
                MaxTimeout = 60000
            };

            var client = new HttpClient(handler);
            try
            {
                await client.GetAsync(uri);
                Assert.Fail("Exception not thrown");
            }
            catch (HttpRequestException e)
            {
                Assert.IsTrue(e.Message.Contains("Name or service not know"));
            }
            catch (Exception e)
            {
                Assert.Fail("Unexpected exception: " + e);
            }
        }

        [TestMethod]
        public async Task SolveErrorBadConfig()
        {
            var handler = new ClearanceHandler("http://localhost:44445")
            {
                MaxTimeout = 60000
            };

            var client = new HttpClient(handler);
            try
            {
                await client.GetAsync(Settings.ProtectedUri);
                Assert.Fail("Exception not thrown");
            }
            catch (FlareSolverrException e)
            {
                Assert.IsTrue(e.Message.Contains("Error connecting to FlareSolverr server"));
            }
            catch (Exception e)
            {
                Assert.Fail("Unexpected exception: " + e);
            }
        }

        [TestMethod]
        public void SolveErrorBadConfigMalformed()
        {
            try
            {
                new ClearanceHandler("http:/127.0.0.1:9999")
                {
                    MaxTimeout = 100
                };
                Assert.Fail("Exception not thrown");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Assert.IsTrue(e.Message.Contains("FlareSolverr URL is malformed: http:/127.0.0.1:9999"));
            }
        }

        [TestMethod]
        public async Task SolveErrorNoConfig()
        {
            var handler = new ClearanceHandler("")
            {
                MaxTimeout = 60000
            };

            var client = new HttpClient(handler);
            try
            {
                await client.GetAsync(Settings.ProtectedUri);
                Assert.Fail("Exception not thrown");
            }
            catch (FlareSolverrException e)
            {
                Assert.IsTrue(e.Message.Contains("Challenge detected but FlareSolverr is not configured"));
            }
            catch (Exception e)
            {
                Assert.Fail("Unexpected exception: " + e);
            }
        }

        [TestMethod]
        public async Task SolveErrorProxy()
        {
            var handler = new ClearanceHandler(Settings.FlareSolverrApiUrl)
            {
                MaxTimeout = 60000,
                ProxyUrl = "http://localhost:44445"
            };

            var client = new HttpClient(handler);
            try
            {
                await client.GetAsync(Settings.ProtectedUri);
                Assert.Fail("Exception not thrown");
            }
            catch (HttpRequestException e)
            {
                Assert.IsTrue(e.Message.Contains("FlareSolverr was unable to process the request, please check FlareSolverr logs. Message: Error: Unable to process browser request. Error: NS_ERROR_PROXY_CONNECTION_REFUSED at " + Settings.ProtectedUri));
            }
            catch (Exception e)
            {
                Assert.Fail("Unexpected exception: " + e);
            }
        }

        [TestMethod]
        public async Task SolveErrorTimeout()
        {
            var handler = new ClearanceHandler(Settings.FlareSolverrApiUrl)
            {
                MaxTimeout = 200
            };

            var client = new HttpClient(handler);
            try
            {
                await client.GetAsync(Settings.ProtectedUri);
                Assert.Fail("Exception not thrown");
            }
            catch (HttpRequestException e)
            {
                Assert.IsTrue(e.Message.Contains("FlareSolverr was unable to process the request, please check FlareSolverr logs. Message: Error: Error solving the challenge. Timeout after 0.2 seconds."));
            }
            catch (Exception e)
            {
                Assert.Fail("Unexpected exception: " + e);
            }
        }

        static ByteArrayContent FormUrlEncodedContentWithEncoding(
            IEnumerable<KeyValuePair<string, string>> nameValueCollection, Encoding encoding)
        {
            // utf-8 / default
            if (Encoding.UTF8.Equals(encoding) || encoding == null)
                return new FormUrlEncodedContent(nameValueCollection);

            // other encodings
            var builder = new StringBuilder();
            foreach (var pair in nameValueCollection)
            {
                if (builder.Length > 0)
                    builder.Append('&');
                builder.Append(HttpUtility.UrlEncode(pair.Key, encoding));
                builder.Append('=');
                builder.Append(HttpUtility.UrlEncode(pair.Value, encoding));
            }
            // HttpRuleParser.DefaultHttpEncoding == "latin1"
            var data = Encoding.GetEncoding("latin1").GetBytes(builder.ToString());
            var content = new ByteArrayContent(data);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
            return content;
        }
    }
}