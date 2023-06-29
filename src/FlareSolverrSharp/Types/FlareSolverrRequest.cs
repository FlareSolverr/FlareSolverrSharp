using System.Text.Json.Serialization;

namespace FlareSolverrSharp.Types
{
    [JsonDerivedType(typeof(FlareSolverrRequestGet))]
    [JsonDerivedType(typeof(FlareSolverrRequestPost))]
    public class FlareSolverrRequest
    {
        [JsonPropertyName("cmd")]
        public string Cmd { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("session")]
        public string Session { get; set; }

        [JsonPropertyName("proxy")]
        public FlareSolverrRequestProxy Proxy { get; set; }
    }
}
