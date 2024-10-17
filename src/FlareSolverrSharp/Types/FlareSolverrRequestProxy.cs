using System.Text.Json.Serialization;

namespace FlareSolverrSharp.Types;

public class FlareSolverrRequestProxy
{
	[JsonPropertyName("url")]
	public string Url;

	[JsonPropertyName("username")]
	public string Username;

	[JsonPropertyName("password")]
	public string Password;
}