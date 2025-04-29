// Author: Deci | Project: FlareSolverrSharp | Name: FlareSolverrSolution.cs
// Date: 2025/04/29 @ 11:04:51

using System.Text.Json.Serialization;

namespace FlareSolverrSharp.Types;

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

	// [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
	public FlareSolverrCookie[] Cookies { get; set; }

	[JsonPropertyName("userAgent")]
	public string UserAgent { get; set; }

	/*[JsonConstructor]
	public FlareSolverrSolution()
	{
		Cookies = [];
	}*/

}