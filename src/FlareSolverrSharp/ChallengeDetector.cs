using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FlareSolverrSharp.Constants;

namespace FlareSolverrSharp;

public static class ChallengeDetector
{

	/// <summary>
	/// Checks if clearance is required.
	/// </summary>
	/// <param name="response">The HttpResponseMessage to check.</param>
	/// <returns>True if the site requires clearance</returns>
	public static Task<bool> IsClearanceRequiredAsync(HttpResponseMessage response)
		=> IsCloudflareProtectedAsync(response);

	/// <summary>
	/// Checks if the site is protected by Cloudflare
	/// </summary>
	/// <param name="response">The HttpResponseMessage to check.</param>
	/// <returns>True if the site is protected</returns>
	private static async Task<bool> IsCloudflareProtectedAsync(HttpResponseMessage response)
	{
		// check response headers
		if (!response.Headers.Server.Any(i =>
			                                 i.Product != null
			                                 && CloudflareValues.CloudflareServerNames.Contains(i.Product.Name.ToLower())))
			return false;

		// detect CloudFlare and DDoS-GUARD
		if (response.StatusCode.Equals(HttpStatusCode.ServiceUnavailable) ||
		    response.StatusCode.Equals(HttpStatusCode.Forbidden)) {
			var responseHtml = await response.Content.ReadAsStringAsync().ConfigureAwait(false);


			if (CloudflareValues.CloudflareBlocked.Any(responseHtml.Contains) || // Cloudflare Blocked
			    responseHtml.Trim().Equals(CloudflareValues.CLOUDFLARE_ERROR_CODE)     || // Cloudflare Blocked
			    responseHtml.IndexOf(CloudflareValues.DDOS_GUARD_TITLE, StringComparison.OrdinalIgnoreCase)
			    > -1) // DDOS-GUARD
				return true;
		}

		// detect Custom CloudFlare for EbookParadijs, Film-Paleis, MuziekFabriek and Puur-Hollands
		if (response.Headers.Vary.ToString()                    == "Accept-Encoding,User-Agent" &&
		    response.Content.Headers.ContentEncoding.ToString() == String.Empty                 &&
		    (await response.Content.ReadAsStringAsync().ConfigureAwait(false)).ToLower().Contains("ddos"))
			return true;

		return false;
	}

}