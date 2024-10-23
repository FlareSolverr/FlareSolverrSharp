using System.Text.Json.Serialization;

namespace FlareSolverrSharp.Types;

public class FlareSolverrRequestProxy
{

	[JsonPropertyName("url")]
	public string Url { get; set; }

	[JsonPropertyName("username")]
	public string Username { get; set; }

	[JsonPropertyName("password")]
	public string Password { get; set; }

	public FlareSolverrRequestProxy() { }

	public FlareSolverrRequestProxy(string url, string username, string password)
	{
		Url      = url;
		Username = username;
		Password = password;
	}

}