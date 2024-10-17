using System.Net;
using System.Text.Json.Serialization;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnassignedField.Global
// ReSharper disable ClassNeverInstantiated.Global
namespace FlareSolverrSharp.Types;

public class FlareSolverrResponse
{

	public string               Status;
	public string               Message;
	public long                 StartTimestamp;
	public long                 EndTimestamp;
	public string               Version;
	public FlareSolverrSolution Solution;
	public string               Session;
	public string[]             Sessions;

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