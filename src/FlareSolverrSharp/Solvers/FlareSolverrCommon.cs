// Author: Deci | Project: FlareSolverrSharp | Name: FlareSolverrCommon.cs
// Date: 2024/10/17 @ 13:10:41

namespace FlareSolverrSharp.Solvers;

public class FlareSolverrCommon
{

	public FlareSolverrCommon(int maxTimeout = FlareSolverr.MAX_TIMEOUT_DEFAULT, string proxyPassword = null,
	                          string proxyUrl = null, string proxyUsername = null)
	{
		MaxTimeout    = maxTimeout;
		ProxyPassword = proxyPassword;
		ProxyUrl      = proxyUrl;
		ProxyUsername = proxyUsername;
	}

	public int MaxTimeout { get; set; } = FlareSolverr.MAX_TIMEOUT_DEFAULT;

	public string ProxyUrl { get; set; }

	public string ProxyUsername { get; set; }

	public string ProxyPassword { get; set; }

}