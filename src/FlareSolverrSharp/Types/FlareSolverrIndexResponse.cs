// Author: Deci | Project: FlareSolverrSharp | Name: FlareSolverrIndexResponse.cs
// Date: 2025/04/29 @ 11:04:27

using System.Text.Json.Serialization;

namespace FlareSolverrSharp.Types;

public class FlareSolverrIndexResponse
{

	[JsonPropertyName("msg")]
	public string Message { get; set; }

	public string Version { get; set; }

	public string UserAgent { get; set; }

}