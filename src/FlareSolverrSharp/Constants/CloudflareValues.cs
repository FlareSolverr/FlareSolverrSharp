namespace FlareSolverrSharp.Constants;

public static class CloudflareValues
{

	public static readonly string[] CloudflareCookiePrefix =
	[
		"cf_",
		"__cf",
		"__ddg"
	];

	public const string CLOUDFLARE_ERROR_CODE_1020   = $"{CLOUDFLARE_ERROR_CODE_PREFIX} 1020";
	public const string CLOUDFLARE_ERROR_CODE_PREFIX = "error code:";

	public const string DDOS_GUARD_TITLE = "<title>DDOS-GUARD</title>";

	public static readonly string[] CloudflareServerNames =
	[
		"cloudflare",
		"cloudflare-nginx",
		"ddos-guard"
	];

	public static readonly string[] CloudflareBlocked =
	[
		"<title>Just a moment...</title>",                // Cloudflare
		"<title>Access denied</title>",                   // Cloudflare Blocked
		"<title>Attention Required! | Cloudflare</title>" // Cloudflare Blocked
	];

	/*
	 *Cloudflare

	   Cloudflare's reverse proxy service expands the 5xx series of errors space to signal issues with the origin server.[51]

	   520 Web Server Returned an Unknown Error
	       The origin server returned an empty, unknown, or unexpected response to Cloudflare.[52]
	   521 Web Server Is Down
	       The origin server refused connections from Cloudflare. Security solutions at the origin may be blocking legitimate connections from certain Cloudflare IP addresses.
	   522 Connection Timed Out
	       Cloudflare timed out contacting the origin server.
	   523 Origin Is Unreachable
	       Cloudflare could not reach the origin server; for example, if the DNS records for the origin server are incorrect or missing.
	   524 A Timeout Occurred
	       Cloudflare was able to complete a TCP connection to the origin server, but did not receive a timely HTTP response.
	   525 SSL Handshake Failed
	       Cloudflare could not negotiate a SSL/TLS handshake with the origin server.
	   526 Invalid SSL Certificate
	       Cloudflare could not validate the SSL certificate on the origin web server. Also used by Cloud Foundry's gorouter.
	   527 Railgun Error (obsolete)
	       Error 527 indicated an interrupted connection between Cloudflare and the origin server's Railgun server.[53] This error is obsolete as Cloudflare has deprecated Railgun.
	   530
	       Error 530 is returned along with a 1xxx error.[54]
	 */


	public enum CloudflareStatusCodes : int
	{

		WebServerUnknown      = 520,
		WebServerDown         = 521,
		ConnectionTimedOut    = 522,
		OriginUnreachable     = 523,
		TimeoutOccurred       = 524,
		SslHandshakeFailed    = 525,
		InvalidSslCertificate = 526,
		RailgunError          = 527,

	}

}