using System.Text.Json.Serialization;


namespace FlareSolverrSharp.Types;

public class FlareSolverrRequest
{
	[JsonPropertyName("cmd")]
	public string Command;

	[JsonPropertyName("url")]
	public string Url;

	[JsonPropertyName("session")]
	public string Session;

	[JsonPropertyName("proxy")]
	public FlareSolverrRequestProxy Proxy;
}