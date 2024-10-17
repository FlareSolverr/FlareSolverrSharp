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

}