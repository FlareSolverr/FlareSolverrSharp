// Author: Deci | Project: FlareSolverrSharp | Name: FlareSolverrCookie.cs
// Date: 2025/04/29 @ 11:04:39

using System.Net;
using System.Text.Json.Serialization;
using Flurl;
using Flurl.Http;

namespace FlareSolverrSharp.Types;

public class FlareSolverrCookie
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
	public int Expiry { get; set; }

	[JsonPropertyName("httpOnly")]
	public bool HttpOnly { get; set; }

	[JsonPropertyName("secure")]
	public bool Secure { get; set; }

	[JsonPropertyName("sameSite")]
	public string SameSite { get; set; }

	public string ToHeaderValue()
		=> $"{Name}={Value}";

	public Cookie ToCookie()
		=> new(Name, Value, Path, Domain);

	public FlurlCookie ToFlurlCookie(Url originUrl = null)
	{
		return new FlurlCookie(Name, Value, originUrl)
		{
			HttpOnly = this.HttpOnly,
			Secure   = this.Secure,
			Path     = this.Path
		};
	}

	/*[JsonConstructor]
	public FlareSolverrCookie(string name, string value)
	{
		Name  = name;
		Value = value;
	}*/

}