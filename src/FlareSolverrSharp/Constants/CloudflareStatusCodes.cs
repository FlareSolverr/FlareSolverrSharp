// Author: Deci | Project: FlareSolverrSharp | Name: CloudflareStatusCodes.cs
// Date: 2026/11/01 @ 12:11:59

namespace FlareSolverrSharp.Constants;

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