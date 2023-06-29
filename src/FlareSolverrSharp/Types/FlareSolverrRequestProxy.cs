using System.Text.Json.Serialization;

namespace FlareSolverrSharp.Types
{
    public class FlareSolverrRequestProxy
    {
        [JsonPropertyName("url")]
        public string Url  { get; set; }
    }
}
