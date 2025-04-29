// Author: Deci | Project: FlareSolverrSharp | Name: FlareSolverrHeaders.cs
// Date: 2025/04/29 @ 11:04:01

using System.Text.Json.Serialization;

namespace FlareSolverrSharp.Types;

public class FlareSolverrHeaders
{

	[JsonPropertyName("status")]
	public string Status { get; set; }

	[JsonPropertyName("date")]
	public string Date { get; set; }

	[JsonPropertyName("content-type")]
	public string ContentType { get; set; }

}