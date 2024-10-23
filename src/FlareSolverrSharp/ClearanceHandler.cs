global using MN = System.Diagnostics.CodeAnalysis.MaybeNullAttribute;
global using MNW = System.Diagnostics.CodeAnalysis.MaybeNullWhenAttribute;
global using MNNW = System.Diagnostics.CodeAnalysis.MemberNotNullWhenAttribute;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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

// ReSharper disable InconsistentNaming

// ReSharper disable InvalidXmlDocComment

namespace FlareSolverrSharp;

/// <summary>
/// A HTTP handler that transparently manages CloudFlare's protection bypass.
/// </summary>
public class ClearanceHandler : DelegatingHandler
{

	private readonly HttpClient m_client;

	private string m_userAgent;

	public FlareSolverr Solverr { get; }


	[MNNW(true, nameof(Solverr))]
	public bool HasFlareSolverr => Solverr != null;

	private HttpClientHandler HttpClientHandler => InnerHandler.GetInnermostHandler() as HttpClientHandler;

	public bool EnsureResponseIntegrity { get; set; }

	public bool CookieCapacity { get; set; }

	private readonly IFlaresolverrResponseStorage _responseStorage;

	/// <summary>
	/// Creates a new instance of the <see cref="ClearanceHandler"/>.
	/// </summary>
	/// <param name="flareSolverrApiUrl">FlareSolverr API URL. If null or empty it will detect the challenges, but
	/// they will not be solved. Example: "http://localhost:8191/"</param>
	public ClearanceHandler(string api)
		: this(new FlareSolverr(api), new DefaultFlaresolverrResponseStorage()) { }


	public ClearanceHandler(FlareSolverr solverr, IFlaresolverrResponseStorage storage)
		: base(new HttpClientHandler())
	{
		m_client = new HttpClient(new HttpClientHandler
		{
			AllowAutoRedirect      = false,
			AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
			CookieContainer        = new CookieContainer()
		});


		Solverr          = solverr;
		_responseStorage = storage;
	}

	/// <summary>
	/// Sends an HTTP request to the inner handler to send to the server as an asynchronous operation.
	/// </summary>
	/// <param name="request">The HTTP request message to send to the server.</param>
	/// <param name="cancellationToken">A cancellation token to cancel operation.</param>
	/// <returns>The task object representing the asynchronous operation.</returns>
	protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
	                                                             CancellationToken cancellationToken)
	{
		// Init FlareSolverr
		if (!HasFlareSolverr) {
			throw new FlareSolverrException($"{nameof(Solverr)} not initialized");
		}

		// Set the User-Agent if required
		SetUserAgentHeader(request);

		// Perform the original user request
		var response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);

		// Detect if there is a challenge in the response
		/*
		if (ChallengeDetector.IsClearanceRequiredAsync(response)) {

			// Resolve the challenge using FlareSolverr API
			var flareSolverrResponse = await Solverr.SolveAsync(request);

			// Save the FlareSolverr User-Agent for the following requests
			var flareSolverUserAgent = flareSolverrResponse.Solution.UserAgent;

			if (flareSolverUserAgent    != null
			    && flareSolverUserAgent != (request.Headers.UserAgent.ToString())) {
				m_userAgent = flareSolverUserAgent;

				// Set the User-Agent if required
				SetUserAgentHeader(request);
			}

			// Change the cookies in the original request with the cookies provided by FlareSolverr
			InjectCookies(request, flareSolverrResponse);
			response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);

			// Detect if there is a challenge in the response
			if (EnsureResponseIntegrity) {

				if (ChallengeDetector.IsClearanceRequiredAsync(response)) {
					throw new FlareSolverrException("The cookies provided by FlareSolverr are not valid");
				}
			}

			// Add the "Set-Cookie" header in the response with the cookies provided by FlareSolverr
			InjectSetCookieHeader(response, flareSolverrResponse);
		}
		*/
		if (!ChallengeDetector.IsClearanceRequiredAsync(response)) {

			return response;
		}

		var flareSolverrResponse = await _responseStorage.LoadAsync();

		if (flareSolverrResponse != null) {
			// Set user agent
			if (flareSolverrResponse.Solution.UserAgent != null
			    && flareSolverrResponse.Solution.UserAgent !=(request.Headers.UserAgent.ToString())) {
				// Set the User-Agent if required
				m_userAgent = flareSolverrResponse.Solution.UserAgent;
				SetUserAgentHeader(request);
			}

			// Retry request with saved response
			InjectCookies(request, flareSolverrResponse);
			response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);

			if (!ChallengeDetector.IsClearanceRequiredAsync(response)) {
				// Success with saved response
				InjectSetCookieHeader(response, flareSolverrResponse);
				return response;
			}
		}

		// Resolve the challenge using FlareSolverr API
		flareSolverrResponse = await Solverr.SolveAsync(request);

		// Save the FlareSolverr User-Agent for the following requests
		var flareSolverUserAgent = flareSolverrResponse.Solution.UserAgent;

		if (flareSolverUserAgent != null && !flareSolverUserAgent.Equals(request.Headers.UserAgent.ToString()))
		{
			m_userAgent = flareSolverUserAgent;

			// Set the User-Agent if required
			SetUserAgentHeader(request);
		}

		// Change the cookies in the original request with the cookies provided by FlareSolverr
		InjectCookies(request, flareSolverrResponse);
		response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);

		// Detect if there is a challenge in the response
		if (EnsureResponseIntegrity && ChallengeDetector.IsClearanceRequiredAsync(response)) {
			throw new FlareSolverrException("The cookies provided by FlareSolverr are not valid");
		}

		// Add the "Set-Cookie" header in the response with the cookies provided by FlareSolverr
		InjectSetCookieHeader(response, flareSolverrResponse);
		await _responseStorage.SaveAsync(flareSolverrResponse);

		return response;
	}

	private void SetUserAgentHeader(HttpRequestMessage request)
	{
		if (m_userAgent != null) {
			// Overwrite the header
			request.Headers.Remove(FlareSolverrValues.UserAgent);
			request.Headers.Add(FlareSolverrValues.UserAgent, m_userAgent);
		}
	}

	private void InjectCookies(HttpRequestMessage request, FlareSolverrResponse flareSolverrResponse)
	{
		// use only Cloudflare and DDoS-GUARD cookies
		var cookies = flareSolverrResponse.Solution.Cookies ?? []; //todo

		var flareCookies = cookies
			.Where(static cookie => IsCloudflareCookie(cookie.Name))
			.ToList();

		// not using cookies, just add flaresolverr cookies to the header request
		if (!HttpClientHandler.UseCookies) {
			foreach (var rCookie in flareCookies) {
				request.Headers.Add(FlareSolverrValues.Cookie, rCookie.ToHeaderValue());
			}

			return;
		}

		var currentCookies = HttpClientHandler.CookieContainer.GetCookies(request.RequestUri);

		// remove previous FlareSolverr cookies
		var oldCookies = flareCookies.Select(flareCookie => currentCookies[flareCookie.Name])
			.Where(static cookie => cookie != null);

		foreach (var cookie in oldCookies) {
			cookie.Expired = true;
		}

		// add FlareSolverr cookies to CookieContainer
		foreach (var rCookie in flareCookies) {
			HttpClientHandler.CookieContainer.Add(request.RequestUri, rCookie.ToCookie());
		}


		if (CookieCapacity) {
			// check if there is too many cookies, we may need to remove some
			if (HttpClientHandler.CookieContainer.PerDomainCapacity >= currentCookies.Count)
				return;

			// check if indeed we have too many cookies
			var validCookiesCount = currentCookies.Cast<Cookie>().Count(cookie => !cookie.Expired);

			if (HttpClientHandler.CookieContainer.PerDomainCapacity >= validCookiesCount)
				return;

			// if there is a too many cookies, we have to make space
			// maybe is better to raise an exception?
			var cookieExcess = HttpClientHandler.CookieContainer.PerDomainCapacity - validCookiesCount;

			foreach (Cookie cookie in currentCookies) {
				if (cookieExcess == 0)
					break;

				if (cookie.Expired || IsCloudflareCookie(cookie.Name))
					continue;

				cookie.Expired =  true;
				cookieExcess   -= 1;
			}

		}
	}

	private static void InjectSetCookieHeader(HttpResponseMessage response,
	                                          FlareSolverrResponse flareSolverrResponse)
	{
		// inject set-cookie headers in the response
		foreach (var rCookie in flareSolverrResponse.Solution.Cookies.Where(
			         cookie => IsCloudflareCookie(cookie.Name))) {
			response.Headers.Add(FlareSolverrValues.SetCookie, rCookie.ToHeaderValue());
		}
	}

	private static bool IsCloudflareCookie(string cookieName)
		=> CloudflareValues.CloudflareCookiePrefix.Any(cookieName.StartsWith);

	protected override void Dispose(bool disposing)
	{
		if (disposing)
			m_client.Dispose();

		base.Dispose(disposing);
	}

}