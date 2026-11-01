using System.Text.Json.Serialization;
using System.Threading.Tasks;


namespace FlareSolverrSharp.Types;

[JsonDerivedType(typeof(FlareSolverrRequestGet))]
[JsonDerivedType(typeof(FlareSolverrRequestPost))]
public class FlareSolverrRequest
{

	[JsonPropertyName("cmd")]
	public string Command { get; set; }

	[JsonPropertyName("url")]
	public string Url { get; set; }

	[JsonPropertyName("session")]
	public string Session { get; set; }

	[JsonPropertyName("proxy")]
	public FlareSolverrRequestProxy Proxy { get; set; }

	[JsonPropertyName("cookies")]
	public FlareSolverrCookie[] Cookies { get; set; }

}