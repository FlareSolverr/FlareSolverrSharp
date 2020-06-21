using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace FlareSolverrSharp
{
    public static class ChallengeDetector
    {
        private static readonly HashSet<string> CloudflareServerNames = new HashSet<string>{"cloudflare", "cloudflare-nginx"};

        /// <summary>
        /// Checks if clearance is required.
        /// </summary>
        /// <param name="response">The HttpResponseMessage to check.</param>
        /// <returns>True if the site requires clearance</returns>
        public static bool IsClearanceRequired(HttpResponseMessage response) => IsCloudflareProtected(response);

        /// <summary>
        /// Checks if the site is protected by Cloudflare
        /// </summary>
        /// <param name="response">The HttpResponseMessage to check.</param>
        /// <returns>True if the site is protected</returns>
        private static bool IsCloudflareProtected(HttpResponseMessage response)
        {
            // check status code
            if (response.StatusCode.Equals(HttpStatusCode.ServiceUnavailable) ||
                response.StatusCode.Equals(HttpStatusCode.Forbidden))
            {
                // check response headers
                return response.Headers.Server.Any(i =>
                    i.Product != null && CloudflareServerNames.Contains(i.Product.Name.ToLower()));
            }
            return false;
        }

    }
}