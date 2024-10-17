using Flurl.Http;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnassignedField.Global
// ReSharper disable ClassNeverInstantiated.Global
namespace FlareSolverrSharp.Types;

public class StringOrNumberConverter : JsonConverter<string>
{

	public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		if (reader.TokenType == JsonTokenType.Number) {
			return reader.GetInt32().ToString();
		}
		else if (reader.TokenType == JsonTokenType.String) {
			return reader.GetString();
		}

		throw new JsonException("Unexpected token type");
	}

	public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
	{
		writer.WriteStringValue(value);
	}

}

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
	public string Version { get; set; }

	[JsonPropertyName("session")]
	public string Session {get;set;}
	[JsonPropertyName("sessions")]
	public string[] Sessions {get;set;}
}

public class FlareSolverrSolution
{

	public string               Url;
	public string               Status;
	public FlareSolverrHeaders  Headers;
	public string               Response;
	public FlareSolverrCookie[] Cookies;
	public string               UserAgent;

}

public class FlareSolverrCookie
{

	public string Name;
	public string Value;
	public string Domain;
	public string Path;
	public double Expires;
	public int    Size;
	public bool   HttpOnly;
	public bool   Secure;
	public bool   Session;
	public string SameSite;

	public string ToHeaderValue()
		=> $"{Name}={Value}";

	public Cookie ToCookie()
		=> new(Name, Value, Path, Domain);

}

public class FlareSolverrHeaders
{

	public string Status;
	public string Date;

	[JsonPropertyName("content-type")]
	public string ContentType;

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