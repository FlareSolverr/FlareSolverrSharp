namespace FlareSolverrSharp.Constants;

public static class CloudflareValues
{

	public const string UserAgent = "User-Agent";

	public const string Cookie = "Cookie";

	public const string SetCookie = "Set-Cookie";

	public static readonly string[] CloudflareCookiePrefix =
	[
		"cf_",
		"__cf",
		"__ddg"
	];

	public const string CLOUDFLARE_ERROR_CODE_1020 = "error code: 1020";
	public const string CLOUDFLARE_ERROR_CODE_PREFIX = "error code: ";

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

	#region 

	public const string CMD_SESSIONS_CREATE = "sessions.create";

	public const string CMD_SESSIONS_LIST = "sessions.list";

	public const string CMD_SESSIONS_DESTROY = "sessions.destroy";

	public const  string CMD_REQUEST_GET     = "request.get";

	public const string CMD_REQUEST_POST    = "request.post";

	#endregion

}