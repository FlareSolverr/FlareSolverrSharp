using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.Json.Serialization;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnassignedField.Global
// ReSharper disable ClassNeverInstantiated.Global
namespace FlareSolverrSharp.Types;

public class FlareSolverrResponse
{

	[JsonPropertyName("solution")]
	public FlareSolverrSolution Solution { get; set; }

	[JsonPropertyName("status")]
	public string Status { get; set; }

	[JsonPropertyName("message")]
	public string Message { get; set; }

	[JsonPropertyName("startTimestamp")]
	public long StartTimestamp { get; set; }

	[JsonPropertyName("endTimestamp")]
	public long EndTimestamp { get; set; }

	[JsonPropertyName("version")]
	public Version Version { get; set; }

	[JsonPropertyName("session")]
	public string Session { get; set; }

	[JsonPropertyName("sessions")]
	public string[] Sessions { get; set; }

}


/*
#region API Objects

public class FlareSolverrRequestProxy
{

	[JsonProperty("url")]
	public string Url;

	[JsonProperty("username")]
	public string Username;

	[JsonProperty("password")]
	public string Password;

}

public record FlareSolverrRequest
{

	// todo

	[JsonPropertyName("cmd")]
	public string Command { get; set; }

	public List<FlareSolverrCookie> Cookies { get; set; }

	public int MaxTimeout { get; set; }

	public FlareSolverrRequestProxy Proxy { get; set; } //todo

	public string Session { get; set; }

	[JsonPropertyName("session_ttl_minutes")]
	public int SessionTtl { get; set; }

	public string Url { get; set; }

	public string PostData { get; set; }

	public bool ReturnOnlyCookies { get; set; }


	public FlareSolverrRequest() { }

}

public class FlareSolverrCookie : IBrowserCookie
{

	[JsonPropertyName("name")]
	public string Name { get; set; }

	[JsonPropertyName("value")]
	public string Value { get; set; }

	[JsonPropertyName("domain")]
	public string Domain { get; set; }

	[JsonPropertyName("path")]
	public string Path { get; set; }

	[JsonPropertyName("expires")]
	public double Expires { get; set; }

	[JsonPropertyName("size")]
	public int Size { get; set; }

	[JsonPropertyName("httpOnly")]
	public bool HttpOnly { get; set; }

	[JsonPropertyName("secure")]
	public bool Secure { get; set; }

	[JsonPropertyName("session")]
	public bool Session { get; set; }

	[JsonPropertyName("sameSite")]
	public string SameSite { get; set; }

	public Cookie AsCookie()
	{
		return new Cookie(Name, Value, Path, Domain)
		{
			Secure   = Secure,
			HttpOnly = HttpOnly,
		};
	}

	public FlurlCookie AsFlurlCookie()
	{
		return new FlurlCookie(Name, Value)
		{
			Domain   = Domain,
			HttpOnly = HttpOnly,
			Path     = Path,
			SameSite = Enum.Parse<SameSite>(SameSite),
			Secure   = Secure
		};
	}


	public string ToHeaderValue()
		=> $"{Name}={Value}";

}

public class FlareSolverrHeaders
{

	[JsonPropertyName("status")]
	public string Status { get; set; }

	[JsonPropertyName("date")]
	public string Date { get; set; }

	[JsonPropertyName("expires")]
	public string Expires { get; set; }

	[JsonPropertyName("cache-control")]
	public string CacheControl { get; set; }

	[JsonPropertyName("content-type")]
	public string ContentType { get; set; }

	[JsonPropertyName("strict-transport-security")]
	public string StrictTransportSecurity { get; set; }

	[JsonPropertyName("p3p")]
	public string P3p { get; set; }

	[JsonPropertyName("content-encoding")]
	public string ContentEncoding { get; set; }

	[JsonPropertyName("server")]
	public string Server { get; set; }

	[JsonPropertyName("content-length")]
	public string ContentLength { get; set; }

	[JsonPropertyName("x-xss-protection")]
	public string XXssProtection { get; set; }

	[JsonPropertyName("x-frame-options")]
	public string XFrameOptions { get; set; }

	[JsonPropertyName("set-cookie")]
	public string SetCookie { get; set; }

}

public class FlareSolverrRoot
{

	[JsonPropertyName("solution")]
	public FlareSolverrSolution Solution { get; set; }

	[JsonPropertyName("status")]
	public string Status { get; set; }

	[JsonPropertyName("message")]
	public string Message { get; set; }

	[JsonPropertyName("startTimestamp")]
	public long StartTimestamp { get; set; }

	[JsonPropertyName("endTimestamp")]
	public long EndTimestamp { get; set; }

	[JsonPropertyName("version")]
	public string Version { get; set; }

}

public class FlareSolverrSolution
{

	[JsonPropertyName("url")]
	public string Url { get; set; }

	[JsonPropertyName("status")]
	public int Status { get; set; }

	[JsonPropertyName("headers")]
	public FlareSolverrHeaders Headers { get; set; }

	[JsonPropertyName("response")]
	public string Response { get; set; }

	[JsonPropertyName("cookies")]
	public List<FlareSolverrCookie> Cookies { get; set; }

	[JsonPropertyName("userAgent")]
	public string UserAgent { get; set; }

}

#endregion
*/