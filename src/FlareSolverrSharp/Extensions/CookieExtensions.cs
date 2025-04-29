// Author: Deci | Project: FlareSolverrSharp | Name: CookieExtensions.cs
// Date: 2025/04/29 @ 11:04:13

using FlareSolverrSharp.Types;
using Flurl;
using Flurl.Http;

namespace FlareSolverrSharp.Extensions;

public static class CookieExtensions
{

	public static FlurlCookie ToFlurlCookie(this FlareSolverrCookie fsc, Url originUrl = null)
	{
		return new FlurlCookie(fsc.Name, fsc.Value, originUrl)
		{
			HttpOnly = fsc.HttpOnly,
			Secure   = fsc.Secure,
			Path     = fsc.Path
		};
	}

}