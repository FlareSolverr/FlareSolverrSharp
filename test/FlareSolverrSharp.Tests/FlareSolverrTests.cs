using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using FlareSolverrSharp.Constants;
using FlareSolverrSharp.Exceptions;
using FlareSolverrSharp.Solvers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FlareSolverrSharp.Tests
{
    [TestClass]
    public class FlareSolverrTests
    {
        [TestMethod]
        public async Task SolveOk()
        {
            var uri = new Uri("https://www.google.com/");
            var flareSolverr = new FlareSolverr(Settings.FlareSolverrApiUrl);
            var request = new HttpRequestMessage(HttpMethod.Get, uri);

            var flareSolverrResponse = await flareSolverr.Solve(request);
            Assert.AreEqual("ok", flareSolverrResponse.Status);
            Assert.AreEqual("", flareSolverrResponse.Message);
            Assert.IsTrue(flareSolverrResponse.StartTimestamp > 0);
            Assert.IsTrue(flareSolverrResponse.EndTimestamp > flareSolverrResponse.StartTimestamp);
            Assert.IsTrue(flareSolverrResponse.Version.Contains("2."));

            Assert.AreEqual("https://www.google.com/", flareSolverrResponse.Solution.Url);
            Assert.IsTrue(flareSolverrResponse.Solution.Response.Contains("<title>Google</title>"));
            Assert.IsTrue(flareSolverrResponse.Solution.Cookies.Any());
            Assert.IsTrue(flareSolverrResponse.Solution.UserAgent.Contains("Firefox/"));

            var firstCookie = flareSolverrResponse.Solution.Cookies.First();
            Assert.IsTrue(!string.IsNullOrWhiteSpace(firstCookie.Name));
            Assert.IsTrue(!string.IsNullOrWhiteSpace(firstCookie.Value));
        }

        [TestMethod]
        public async Task SolveOkUserAgent()
        {
            const string userAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/81.0.4044.138 Safari/537.36";
            var uri = new Uri("https://www.google.com/");
            var flareSolverr = new FlareSolverr(Settings.FlareSolverrApiUrl);
            var request = new HttpRequestMessage(HttpMethod.Get, uri);
            request.Headers.Add(HttpHeaders.UserAgent, userAgent);

            var flareSolverrResponse = await flareSolverr.Solve(request);
            Assert.AreEqual("ok", flareSolverrResponse.Status);
            Assert.IsTrue(flareSolverrResponse.Solution.UserAgent.Contains("Firefox/"));
        }

        [TestMethod]
        public async Task SolveOkProxy()
        {
            var uri = new Uri("https://www.google.com/");
            var flareSolverr = new FlareSolverr(Settings.FlareSolverrApiUrl)
            {
                ProxyUrl = Settings.ProxyUrl
            };
            var request = new HttpRequestMessage(HttpMethod.Get, uri);

            var flareSolverrResponse = await flareSolverr.Solve(request);
            Assert.AreEqual("ok", flareSolverrResponse.Status);
            Assert.AreEqual("", flareSolverrResponse.Message);
            Assert.IsTrue(flareSolverrResponse.StartTimestamp > 0);
            Assert.IsTrue(flareSolverrResponse.EndTimestamp > flareSolverrResponse.StartTimestamp);
            Assert.IsTrue(flareSolverrResponse.Version.Contains("2."));

            Assert.AreEqual("https://www.google.com/", flareSolverrResponse.Solution.Url);
            Assert.IsTrue(flareSolverrResponse.Solution.Response.Contains("<title>Google</title>"));
            Assert.IsTrue(flareSolverrResponse.Solution.Cookies.Any());
            Assert.IsTrue(flareSolverrResponse.Solution.UserAgent.Contains("Firefox/"));

            var firstCookie = flareSolverrResponse.Solution.Cookies.First();
            Assert.IsTrue(!string.IsNullOrWhiteSpace(firstCookie.Name));
            Assert.IsTrue(!string.IsNullOrWhiteSpace(firstCookie.Value));
        }

        [TestMethod]
        public async Task SolveErrorUrl()
        {
            var uri = new Uri("https://www.google.bad1/");
            var flareSolverr = new FlareSolverr(Settings.FlareSolverrApiUrl);
            var request = new HttpRequestMessage(HttpMethod.Get, uri);

            try
            {
                await flareSolverr.Solve(request);
                Assert.Fail("Exception not thrown");
            }
            catch (FlareSolverrException e)
            {
                Assert.AreEqual("FlareSolverr was unable to process the request, please check FlareSolverr logs. Message: Error: Unable to process browser request. Error: NS_ERROR_UNKNOWN_HOST at https://www.google.bad1/", e.Message);
            }
            catch (Exception e)
            {
                Assert.Fail("Unexpected exception: " + e);
            }
        }

        [TestMethod]
        public async Task SolveErrorConfig()
        {
            var uri = new Uri("https://www.google.com/");
            var flareSolverr = new FlareSolverr("http://localhost:44445");
            var request = new HttpRequestMessage(HttpMethod.Get, uri);

            try
            {
                await flareSolverr.Solve(request);
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
        public async Task SolveErrorProxy()
        {
            var uri = new Uri("https://www.google.com/");
            var flareSolverr = new FlareSolverr(Settings.FlareSolverrApiUrl)
            {
                ProxyUrl = "http://localhost:44445"
            };
            var request = new HttpRequestMessage(HttpMethod.Get, uri);

            try
            {
                await flareSolverr.Solve(request);
                Assert.Fail("Exception not thrown");
            }
            catch (FlareSolverrException e)
            {
                Assert.IsTrue(e.Message.Contains("FlareSolverr was unable to process the request, please check FlareSolverr logs. Message: Error: Unable to process browser request. Error: NS_ERROR_PROXY_CONNECTION_REFUSED at https://www.google.com/"));
            }
            catch (Exception e)
            {
                Assert.Fail("Unexpected exception: " + e);
            }
        }

        [TestMethod]
        public async Task SolveErrorTimeout()
        {
            var uri = new Uri("https://www.google.com/");
            var flareSolverr = new FlareSolverr(Settings.FlareSolverrApiUrl)
            {
                MaxTimeout = 100
            };
            var request = new HttpRequestMessage(HttpMethod.Get, uri);

            try
            {
                await flareSolverr.Solve(request);
                Assert.Fail("Exception not thrown");
            }
            catch (FlareSolverrException e)
            {
                Assert.IsTrue(e.Message.Contains("FlareSolverr was unable to process the request, please check FlareSolverr logs. Message: Error: Unable to process browser request. Error: Maximum timeout reached. maxTimeout=100 (ms)"));
            }
            catch (Exception e)
            {
                Assert.Fail("Unexpected exception: " + e);
            }
        }
        [TestMethod]
        public async Task SolveTestSessions()
        {
            // create new session
            var flareSolverr = new FlareSolverr(Settings.FlareSolverrApiUrl);
            var flareSolverrResponse = await flareSolverr.CreateSession();

            Assert.AreEqual("ok", flareSolverrResponse.Status);
            Assert.AreEqual("Session created successfully.", flareSolverrResponse.Message);
            Assert.IsTrue(flareSolverrResponse.StartTimestamp > 0);
            Assert.IsTrue(flareSolverrResponse.EndTimestamp > flareSolverrResponse.StartTimestamp);
            Assert.IsTrue(flareSolverrResponse.Version.Contains("2."));
            Assert.IsTrue(flareSolverrResponse.Session.Length > 0);

            // request with session
            var sessionId = flareSolverrResponse.Session;
            var uri = new Uri("https://www.google.com/");
            var request = new HttpRequestMessage(HttpMethod.Get, uri);
            flareSolverrResponse = await flareSolverr.Solve(request, sessionId);
            Assert.AreEqual("ok", flareSolverrResponse.Status);
            Assert.AreEqual("200", flareSolverrResponse.Solution.Status);

            // list sessions
            flareSolverrResponse = await flareSolverr.ListSessions();
            Assert.AreEqual("ok", flareSolverrResponse.Status);
            Assert.IsTrue(flareSolverrResponse.Sessions.Contains(sessionId));

            // destroy session
            flareSolverrResponse = await flareSolverr.DestroySession(sessionId);
            Assert.AreEqual("ok", flareSolverrResponse.Status);
        }
    }
}