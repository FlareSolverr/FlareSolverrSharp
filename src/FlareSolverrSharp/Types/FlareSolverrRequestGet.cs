using System.Text.Json.Serialization;


namespace FlareSolverrSharp.Types;

public class FlareSolverrRequestGet : FlareSolverrRequest
{
	[JsonPropertyName("maxTimeout")]
	public int MaxTimeout {get;set;}
}